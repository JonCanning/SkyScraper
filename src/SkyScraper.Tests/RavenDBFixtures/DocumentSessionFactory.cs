using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Listeners;

namespace SkyScraper.Tests.RavenDBFixtures
{
    public static class DocumentSessionFactory
    {
        static EmbeddableDocumentStore documentStore;

        public static IDocumentSession CreateNewStore()
        {
            documentStore = new EmbeddableDocumentStore { RunInMemory = true, Conventions = { DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites } };
            documentStore.RegisterListener(new NoStaleQueriesListener());
            documentStore.Initialize();
            return GetSessionFromStore();
        }

        public static IDocumentSession GetSessionFromStore()
        {
            return documentStore.OpenSession();
        }

        class NoStaleQueriesListener : IDocumentQueryListener
        {
            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }
        }
    }
}