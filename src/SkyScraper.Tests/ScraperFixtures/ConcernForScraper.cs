using NSubstitute;
using System;

namespace SkyScraper.Tests.ScraperFixtures
{
    abstract class ConcernForScraper : ConcernFor<Scraper>
    {
        protected ITaskRunner TaskRunner;
        protected IHttpClient HttpClient;
        protected Uri Uri;
        protected Action<HtmlDoc> OnNext;

        protected override void Context()
        {
            TaskRunner = new SyncTaskRunner();
            HttpClient = Substitute.For<IHttpClient>();
        }

        protected override Scraper CreateClassUnderTest()
        {
            SUT = new Scraper(TaskRunner, HttpClient);
            SUT.Subscribe(OnNext);
            return SUT;
        }

        protected override void Because()
        {
            SUT.Scrape(Uri);
        }
    }
}