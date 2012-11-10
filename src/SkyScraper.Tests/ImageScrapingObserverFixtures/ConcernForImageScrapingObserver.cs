using NSubstitute;
using SkyScraper.Observers.ImageScraper;

namespace SkyScraper.Tests.ImageScrapingObserverFixtures
{
    abstract class ConcernForImageScrapingObserverOnNext : ConcernFor<ImageScraperObserver>
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

        protected override ImageScraperObserver CreateClassUnderTest()
        {
            return new ImageScraperObserver(TaskRunner, HttpClient, FileWriter);
        }

        protected override void Because()
        {
            SUT.OnNext(HtmlDoc);
        }
    }
}
