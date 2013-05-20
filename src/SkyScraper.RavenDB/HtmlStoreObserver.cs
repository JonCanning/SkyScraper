using System;
using System.Linq;
using Raven.Client;

namespace SkyScraper.RavenDb
{
    public class HtmlStoreObserver : IObserver<HtmlDoc>
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

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}