using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_scrape_has_been_running_for_too_long : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override Scraper CreateClassUnderTest()
        {
            DateTimeProvider.UtcNow = DateTime.Today;
            SUT = base.CreateClassUnderTest();
            SUT.TimeOut = TimeSpan.FromSeconds(0);
            DateTimeProvider.UtcNow = DateTime.Today.AddDays(1);
            return SUT;
        }

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            page = @"<html>
                    <a href=""link1"">link1</a>
                    </html>";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_no_htmldocs_should_be_returned()
        {
            htmlDocs.Count.Should().Be(0);
        }
    }
}