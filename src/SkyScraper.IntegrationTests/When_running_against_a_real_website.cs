using System;
using System.IO;
using NUnit.Framework;
using SkyScraper.Observers.ConsoleWriter;
using SkyScraper.Observers.ImageScraper;

namespace SkyScraper.IntegrationTests
{
    [TestFixture]
    [Explicit]
    class When_running_against_a_real_website
    {
        [Test]
        public void Then_images_should_be_saved()
        {
            var scraper = new Scraper(new HttpClient(), new ScrapedUrisDictionary());
            var io = new ImageScraperObserver(new HttpClient(), new FileWriter(new DirectoryInfo("c:\\temp")));
            scraper.Subscribe(io);
            scraper.Subscribe(new ConsoleWriterObserver());
            scraper.Scrape(new Uri("http://www.cambridgecupcakes.com/")).Wait();
        }
    }
}