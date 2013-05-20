using Raven.Client;
using System;
using System.Linq;

namespace SkyScraper.RavenDb
{
    public class HtmlStoreObserver : IObserver<HtmlDoc>, IScrapedUris
    {
        readonly Func<IDocumentSession> documentSessionFunc;

        public HtmlStoreObserver(Func<IDocumentSession> documentSessionFunc)
        {
            this.documentSessionFunc = documentSessionFunc;
        }

        public void OnNext(HtmlDoc htmlDoc)
        {
            using (var documentSession = documentSessionFunc())
            {
                var storedHtmlDoc = documentSession.Query<HtmlDoc>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).Single(x => x.Uri == htmlDoc.Uri);
                storedHtmlDoc.Html = htmlDoc.Html;
                documentSession.Store(storedHtmlDoc);
                documentSession.SaveChanges();
            }
        }

        public void OnError(Exception error) { }

        public void OnCompleted() { }

        public bool TryAdd(Uri uri)
        {
            using (var documentSession = documentSessionFunc())
            {
                if (documentSession.Query<HtmlDoc>().Any(x => x.Uri == uri))
                    return false;
                var htmlDoc = new HtmlDoc { Uri = uri };
                documentSession.Store(htmlDoc);
                documentSession.SaveChanges();
                return true;
            }
        }
    }
}