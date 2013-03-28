using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_website_returns_exception_from_page : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            page = @"<html>
                         <a href=""page1"">link1</a>
                         </html>";
            HttpClient.GetString(Uri).Returns(new Task<string>(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(new Task(() => { throw new HttpRequestException(); }));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_one_htmldoc_should_be_returned()
        {
            htmlDocs.Count.Should().Be(1);
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page()
        {
            htmlDocs.Should().Contain(x => x.Uri.ToString() == "http://test/" && x.Html == page);
        }

        [Test]
        public void Then_link_should_be_scraped()
        {
            HttpClient.Received().GetString(Arg.Is<Uri>(x => x.ToString() == "http://test/page1"));
        }
    }
}