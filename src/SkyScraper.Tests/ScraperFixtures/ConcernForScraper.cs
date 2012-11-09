using NSubstitute;
using System;
using System.Threading.Tasks;

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
            TaskRunner = new TestTaskRunner();
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

    class TestTaskRunner : ITaskRunner
    {
        public void Run(Action action)
        {
            action();
        }

        public Task Run(Task task)
        {
            task.Start();
            return task;
        }

        public void WaitForAllTasks()
        {

        }
    }
}