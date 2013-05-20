using Raven.Client;
using SkyScraper.RavenDb;

namespace SkyScraper.Tests.RavenDBFixtures.ScrapedUrisFixtures
{
    class ConcernForIScrapedUris : ConcernFor<ScrapedUrisRavenDb>
    {
        protected IDocumentSession DocumentSession;

        protected override ScrapedUrisRavenDb CreateClassUnderTest()
        {
            DocumentSession = DocumentSessionFactory.Create();
            return new ScrapedUrisRavenDb(DocumentSession);
        }
    }
}