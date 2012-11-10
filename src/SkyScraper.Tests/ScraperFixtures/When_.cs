using System;
using System.IO;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture, Explicit]
    class When_
    {
        [Test]
        public void Then_()
        {
            var scraper = new Scraper();
            var io = new ImageScrapingObserver(new FileWriter(new DirectoryInfo("c:\\temp")));
            scraper.Subscribe(io);
            //scraper.Subscribe(x => Console.WriteLine(x.Uri));
            scraper.Scrape(new Uri("http://www.cambridgecupcakes.com/"));
        }
    }
}