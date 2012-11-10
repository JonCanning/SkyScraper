using NUnit.Framework;
using SkyScraper.Observers.ConsoleWriter;
using SkyScraper.Observers.ImageScraper;
using System;
using System.IO;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture, Explicit]
    class When_running_against_a_real_website
    {
        [Test]
        public void Then_images_should_be_saved()
        {
            var scraper = new Scraper();
            var io = new ImageScraperObserver(new FileWriter(new DirectoryInfo("c:\\temp")));
            scraper.Subscribe(io);
            scraper.Subscribe(new ConsoleWriterObserver());
            //scraper.Subscribe(x => Console.WriteLine(x.Uri));
            scraper.Scrape(new Uri("http://www.cambridgecupcakes.com/"));
        }
    }
}