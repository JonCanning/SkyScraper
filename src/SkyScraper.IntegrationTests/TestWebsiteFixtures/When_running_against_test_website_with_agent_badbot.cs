using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SkyScraper.Observers.ConsoleWriter;

namespace SkyScraper.IntegrationTests.TestWebsiteFixtures
{
    [TestFixture]
    class When_running_against_test_website_with_agent_badbot
    {
        [Test]
        public void Then_no_results_should_be_returned()
        {
            var list = new List<HtmlDoc>();
            var httpClient = new HttpClient { UserAgentName = "badbot" };
            var scraper = new Scraper(httpClient, new ScrapedUrisDictionary());
            scraper.Subscribe(new ConsoleWriterObserver());
            scraper.Subscribe(list.Add);
            scraper.Scrape(new Uri("http://localhost:12345")).Wait();
            list.Should().BeEmpty();
        }
    }
}