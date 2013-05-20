using Raven.Client;
using SkyScraper.RavenDb;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures
{
    class ConcernForHtmlStoreObserver : ConcernFor<HtmlStoreObserver>
    {
        protected IDocumentSession DocumentSession;

        protected override HtmlStoreObserver CreateClassUnderTest()
        {
            DocumentSession = DocumentSessionFactory.Create();
            return new HtmlStoreObserver(DocumentSession);
        }
    }
}