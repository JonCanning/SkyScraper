using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class Scraper : IScraper, IObservable<HtmlDoc>
    {
        readonly IHttpClient httpClient;
        readonly IScrapedUris scrapedUris;
        Uri baseUri;

        DateTime? endDateTime;
        public List<IObserver<HtmlDoc>> Observers { get; set; }

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

        public async Task Scrape(Uri uri, TimeSpan timeout)
        {
            endDateTime = DateTimeProvider.UtcNow + timeout;
            await Scrape(uri);
        }

        public async Task Scrape(Uri uri)
        {
            baseUri = uri;
            await DownloadHtml(uri);
        }

        async Task DownloadHtml(Uri uri)
        {
            if (endDateTime.HasValue && endDateTime < DateTimeProvider.UtcNow)
                return;
            if (uri.ToString().Length > 2048)
                return;
            if (!scrapedUris.TryAdd(uri))
                return;
            try
            {
                var html = await httpClient.GetString(uri);
                if (string.IsNullOrEmpty(html))
                    return;
                var htmlDoc = new HtmlDoc { Uri = uri, Html = html };
                NotifyObservers(htmlDoc);
                await ParseLinks(htmlDoc);
            }
            catch { }
        }

        async Task ParseLinks(HtmlDoc htmlDoc)
        {
            var pageBase = htmlDoc.Uri.Segments.Last().Contains('.') ? htmlDoc.Uri.ToString().Substring(0, htmlDoc.Uri.ToString().LastIndexOf('/')) : htmlDoc.Uri.ToString();
            if (!pageBase.EndsWith("/"))
                pageBase += "/";
            var pageBaseUri = new Uri(pageBase);
            CQ html = htmlDoc.Html;
            var links = html["a"].Select(x => x.GetAttribute("href")).Where(x => x != null);
            var localLinks = LocalLinks(links).Select(x => NormalizeLink(x, pageBaseUri));
            foreach (var downloadUri in localLinks)
                await DownloadHtml(downloadUri);
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
            Observers.ForEach(o => o.OnNext(htmlDoc));
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