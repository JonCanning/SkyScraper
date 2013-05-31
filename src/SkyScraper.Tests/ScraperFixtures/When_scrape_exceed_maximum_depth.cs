using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_scrape_exceed_maximum_depth : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override Scraper CreateClassUnderTest()
        {
            SUT = base.CreateClassUnderTest();
            SUT.MaxDepth = 1;
            return SUT;
        }

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            page = @"<html>
                    <a href=""link1"">link1</a>
                    <a href=""link1/link2"">link1</a>
                    </html>";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_two_htmldocs_should_be_returned()
        {
            htmlDocs.Count.Should().Be(2);
        }
    }
}