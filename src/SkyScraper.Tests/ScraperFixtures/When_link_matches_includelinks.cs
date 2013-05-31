using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_link_matches_includelinks : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override Scraper CreateClassUnderTest()
        {
            SUT = base.CreateClassUnderTest();
            SUT.IncludeLinks = new Regex(@"link1");
            return SUT;
        }

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            page = @"<html>
                    <a href=""link1"">link1</a>
                    <a href=""link2"">link2</a>
                    </html>";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_link1_should_be_scraped()
        {
            HttpClient.Received().GetString(Arg.Is<Uri>(x => x.ToString().EndsWith("link1")));
        }

        [Test]
        public void Then_link2_should_not_be_scraped()
        {
            HttpClient.DidNotReceive().GetString(Arg.Is<Uri>(x => x.ToString().EndsWith("link2")));
        }

        [Test]
        public void Then_two_htmldocs_should_be_returned()
        {
            htmlDocs.Count.Should().Be(2);
        }
    }
}