using NSubstitute;

namespace SkyScraper.Tests.ImageScrapingObserverFixtures
{
    abstract class ConcernForImageScrapingObserverOnNext : ConcernFor<ImageScrapingObserver>
    {
        protected ITaskRunner TaskRunner;
        protected IHttpClient HttpClient;
        protected IFileWriter FileWriter;
        protected HtmlDoc HtmlDoc;

        protected override void Context()
        {
            TaskRunner = new SyncTaskRunner();
            HttpClient = Substitute.For<IHttpClient>();
            FileWriter = Substitute.For<IFileWriter>();
            HtmlDoc = new HtmlDoc();
        }

        protected override ImageScrapingObserver CreateClassUnderTest()
        {
            return new ImageScrapingObserver(TaskRunner, HttpClient, FileWriter);
        }

        protected override void Because()
        {
            SUT.OnNext(HtmlDoc);
        }
    }
}
