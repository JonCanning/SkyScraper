using Raven.Client;
using System;
using System.Linq;

namespace SkyScraper.RavenDb
{
    public class HtmlStoreObserver : IObserver<HtmlDoc>, IScrapedUris
    {
        readonly IDocumentSession documentSession;

        public HtmlStoreObserver(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void OnNext(HtmlDoc htmlDoc)
        {
            var storedHtmlDoc = documentSession.Query<HtmlDoc>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).Single(x => x.Uri == htmlDoc.Uri);
            storedHtmlDoc.Html = htmlDoc.Html;
            documentSession.Store(storedHtmlDoc);
        }

        public void OnError(Exception error) { }

        public void OnCompleted() { }

        public bool TryAdd(Uri uri)
        {
            if (documentSession.Query<HtmlDoc>().Any(x => x.Uri == uri))
                return false;
            var htmlDoc = new HtmlDoc { Uri = uri };
            documentSession.Store(htmlDoc);
            return true;
        }
    }
}