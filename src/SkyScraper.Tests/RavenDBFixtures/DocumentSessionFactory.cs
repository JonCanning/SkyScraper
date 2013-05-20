using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Listeners;

namespace SkyScraper.Tests.RavenDBFixtures
{
    public static class DocumentSessionFactory
    {
        public static IDocumentSession Create()
        {
            var documentStore = new EmbeddableDocumentStore { RunInMemory = true, Conventions = { DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites } };
            documentStore.RegisterListener(new NoStaleQueriesListener());
            documentStore.Initialize();
            var documentSession = documentStore.OpenSession();
            return documentSession;
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