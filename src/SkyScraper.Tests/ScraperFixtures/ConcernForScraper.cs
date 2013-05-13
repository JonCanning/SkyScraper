using System.Threading.Tasks;
using NSubstitute;
using System;

namespace SkyScraper.Tests.ScraperFixtures
{
    abstract class ConcernForScraper : ConcernFor<Scraper>
    {
        protected IHttpClient HttpClient;
        protected Uri Uri;
        protected Action<HtmlDoc> OnNext;

        protected override void Context()
        {
            HttpClient = Substitute.For<IHttpClient>();
        }

        protected override Scraper CreateClassUnderTest()
        {
            SUT = new Scraper(HttpClient);
            SUT.Subscribe(OnNext);
            return SUT;
        }

        protected override void Because()
        {
            SUT.Scrape(Uri).Wait();
        }
    }
}