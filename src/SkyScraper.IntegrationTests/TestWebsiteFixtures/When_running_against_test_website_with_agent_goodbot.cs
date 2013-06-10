using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SkyScraper.Observers.ConsoleWriter;
using System.Linq;

namespace SkyScraper.IntegrationTests.TestWebsiteFixtures
{
    [TestFixture]
    class When_running_against_test_website_with_agent_goodbot
    {
        List<HtmlDoc> list;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            list = new List<HtmlDoc>();
            var httpClient = new HttpClient { UserAgentName = "goodbot" };
            var scraper = new Scraper(httpClient, new ScrapedUrisDictionary());
            scraper.Subscribe(new ConsoleWriterObserver());
            scraper.Subscribe(list.Add);
            scraper.Scrape(new Uri("http://localhost:12345")).Wait();
        }

        [Test]
        public void Then_5_results_should_be_returned()
        {
            list.Should().HaveCount(5);
        }

        [Test]
        public void Then_about_page_should_contain_useragentname()
        {
            list.Single(x => x.Uri.ToString().EndsWith("About")).Html.Should().Contain("The UserAgent is goodbot");
        }
    }
}