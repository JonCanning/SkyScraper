using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class Scraper : IScraper, IObservable<HtmlDoc>
    {
        readonly IHttpClient httpClient;
        readonly IScrapedUris scrapedUris;
        Uri baseUri;
        DateTime? endDateTime;
        Action<Exception> onHttpClientException = delegate { };
        bool robotsLoaded;

        public List<IObserver<HtmlDoc>> Observers { get; set; }
        public TimeSpan TimeOut
        {
            set
            {
                endDateTime = DateTimeProvider.UtcNow + value;
            }
        }
        public int? MaxDepth { private get; set; }
        public Regex IgnoreLinks { private get; set; }
        public Regex IncludeLinks { private get; set; }
        public Regex ObserverLinkFilter { private get; set; }
        public bool EnableRobotsProtocol { get; set; }
        public string UserAgentName { get; set; }

        public Action<Exception> OnHttpClientException
        {
            get { return onHttpClientException; }
            set { onHttpClientException = value; }
        }

        public Scraper(IHttpClient httpClient, IScrapedUris scrapedUris)
        {
            this.httpClient = httpClient;
            this.scrapedUris = scrapedUris;
            Observers = new List<IObserver<HtmlDoc>>();
        }

        public IDisposable Subscribe(IObserver<HtmlDoc> observer)
        {
            Observers.Add(observer);
            return new Unsubscriber(Observers, observer);
        }

        public async Task Scrape(Uri uri)
        {
            baseUri = baseUri ?? uri;

            if (EnableRobotsProtocol)
            {
                if (!robotsLoaded)
                {
                    var robotsUri = new Uri(uri.GetLeftPart(UriPartial.Authority) + "/robots.txt");
                    var robotsTxt = await httpClient.GetString(robotsUri);
                    Robots.Load(robotsTxt, UserAgentName);
                    robotsLoaded = true;
                }
                if (!Robots.PathIsAllowed(uri.PathAndQuery))
                    return;
            }


            if (endDateTime.HasValue && DateTimeProvider.UtcNow > endDateTime)
                return;
            if (!scrapedUris.TryAdd(uri))
                return;
            var htmlDoc = new HtmlDoc { Uri = uri };
            try
            {
                htmlDoc.Html = await httpClient.GetString(uri);
            }
            catch (Exception exception)
            {
                OnHttpClientException(exception);
            }
            if (string.IsNullOrEmpty(htmlDoc.Html))
                return;
            if (!(ObserverLinkFilter != null && !ObserverLinkFilter.IsMatch(uri.ToString())))
                NotifyObservers(htmlDoc);

            var pageBase = htmlDoc.Uri.Segments.Last().Contains('.') ? htmlDoc.Uri.ToString().Substring(0, htmlDoc.Uri.ToString().LastIndexOf('/')) : htmlDoc.Uri.ToString();
            if (!pageBase.EndsWith("/"))
                pageBase += "/";
            var pageBaseUri = new Uri(pageBase);
            CQ cq = htmlDoc.Html;
            var links = cq["a"].Select(x => x.GetAttribute("href")).Where(x => x != null);
            var localLinks = LocalLinks(links).Select(x => NormalizeLink(x, pageBaseUri)).Where(x => x.ToString().Length <= 2048);
            if (IncludeLinks != null)
                localLinks = localLinks.Where(x => IncludeLinks.IsMatch(x.ToString()));
            if (IgnoreLinks != null)
                localLinks = localLinks.Where(x => !IgnoreLinks.IsMatch(x.ToString()));
            if (MaxDepth.HasValue)
                localLinks = localLinks.Where(x => x.Segments.Length <= MaxDepth + 1);
            var tasks = localLinks.Select(Scrape).ToArray();
            Task.WaitAll(tasks);
        }

        Uri NormalizeLink(string link, Uri pageBaseUri)
        {
            if (link.StartsWith("/"))
                return new Uri(baseUri, link);
            if (link.StartsWith(baseUri.ToString()))
                return new Uri(link);
            return new Uri(pageBaseUri, link);
        }

        void NotifyObservers(HtmlDoc htmlDoc)
        {
            Observers.ForEach(x => x.OnNext(htmlDoc));
        }

        IEnumerable<string> LocalLinks(IEnumerable<string> links)
        {
            return links.Select(WebUtility.HtmlDecode).Where(x => x.LinkIsLocal(baseUri.ToString()) && x.LinkDoesNotContainAnchor());
        }

        class Unsubscriber : IDisposable
        {
            readonly IObserver<HtmlDoc> observer;
            readonly List<IObserver<HtmlDoc>> observers;

            public Unsubscriber(List<IObserver<HtmlDoc>> observers, IObserver<HtmlDoc> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (observer != null && observers.Contains(observer))
                    observers.Remove(observer);
            }
        }
    }
}