using System;
using Raven.Client;
using System.Linq;

namespace SkyScraper.RavenDb
{
    public class ScrapedUrisRavenDb : IScrapedUris
    {
        readonly IDocumentSession documentSession;

        public ScrapedUrisRavenDb(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

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
