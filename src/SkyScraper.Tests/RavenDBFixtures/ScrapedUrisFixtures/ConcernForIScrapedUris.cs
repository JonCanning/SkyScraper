using Raven.Client;
using SkyScraper.RavenDb;

namespace SkyScraper.Tests.RavenDBFixtures.ScrapedUrisFixtures
{
    class ConcernForIScrapedUris : ConcernFor<HtmlStoreObserver>
    {
        protected IDocumentSession DocumentSession;

        protected override HtmlStoreObserver CreateClassUnderTest()
        {
            DocumentSession = DocumentSessionFactory.Create();
            return new HtmlStoreObserver(DocumentSession);
        }
    }
}