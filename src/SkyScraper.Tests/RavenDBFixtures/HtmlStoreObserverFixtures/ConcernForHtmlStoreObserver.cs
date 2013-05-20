using SkyScraper.RavenDb;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures
{
    class ConcernForHtmlStoreObserver : ConcernFor<HtmlStoreObserver>
    {
        protected override HtmlStoreObserver CreateClassUnderTest()
        {
            return new HtmlStoreObserver(DocumentSessionFactory.GetSessionFromStore);
        }
    }
}