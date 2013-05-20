using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class Scraper : IObservable<HtmlDoc>
    {
        readonly IHttpClient httpClient;
        readonly IScrapedUris scrapedUris;
        readonly List<IObserver<HtmlDoc>> observers = new List<IObserver<HtmlDoc>>();
        Uri baseUri;

        public Scraper(IHttpClient httpClient, IScrapedUris scrapedUris)
        {
            this.httpClient = httpClient;
            this.scrapedUris = scrapedUris;
        }

        public IDisposable Subscribe(IObserver<HtmlDoc> observer)
        {
            observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        public async Task Scrape(Uri uri)
        {
            baseUri = uri;
            await DownloadDocument(uri);
        }

        async Task DownloadDocument(Uri uri)
        {
            if (!scrapedUris.TryAdd(uri))
                return;
            try
            {
                var html = await httpClient.GetString(uri);
                await StoreHtmlDoc(uri, html);
            }
            catch { }
        }

        async Task StoreHtmlDoc(Uri uri, string html)
        {
            var htmlDoc = new HtmlDoc { Uri = uri, Html = html };
            observers.ForEach(o => o.OnNext(htmlDoc));
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var linkNodeCollection = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodeCollection == null || !linkNodeCollection.Any())
                return;
            var localLinks = LocalLinks(linkNodeCollection);
            var pageBaseUri = htmlDoc.Uri.Segments.Last().Contains('.') ? htmlDoc.Uri.ToString().Substring(0, htmlDoc.Uri.ToString().LastIndexOf('/')) : htmlDoc.Uri.ToString();
            if (pageBaseUri.Last() != '/')
                pageBaseUri += '/';
            foreach (var downloadUri in localLinks.Select(href => new Uri(new Uri(pageBaseUri), href)))
                await DownloadDocument(downloadUri);
        }

        IEnumerable<string> LocalLinks(IEnumerable<HtmlNode> linkNodeCollection)
        {
            return linkNodeCollection.Select(x => WebUtility.HtmlDecode(x.Attributes["href"].Value)).Where(x => x.LinkIsLocal(baseUri.ToString()) && x.LinkDoesNotContainAnchor());
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