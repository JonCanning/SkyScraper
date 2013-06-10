using NSubstitute;
using System;

namespace SkyScraper.Tests.ScraperFixtures
{
    abstract class ConcernForScraper : ConcernFor<Scraper>
    {
        protected IHttpClient HttpClient;
        protected Action<HtmlDoc> OnNext;
        protected Uri Uri;

        protected override void Context()
        {
            HttpClient = Substitute.For<IHttpClient>();
        }

        protected override Scraper CreateClassUnderTest()
        {
            SUT = new Scraper(HttpClient, new ScrapedUrisDictionary());
            SUT.DisableRobotsProtocol = true;
            SUT.Subscribe(OnNext);
            return SUT;
        }

        protected override void Because()
        {
            SUT.Scrape(Uri).Wait();
        }
    }
}