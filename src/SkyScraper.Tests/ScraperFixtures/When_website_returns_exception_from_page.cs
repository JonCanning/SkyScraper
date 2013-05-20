using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_website_returns_exception_from_page : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        const string Page = @"<html><a href=""page1"">link1</a></html>";

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => Page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(Task.Run(() => { throw new HttpRequestException(); }));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page()
        {
            htmlDocs.Should().Contain(x => x.Uri.ToString() == "http://test/" && x.Html == Page);
        }

        [Test]
        public void Then_link_should_be_scraped()
        {
            HttpClient.Received().GetString(Arg.Is<Uri>(x => x.ToString() == "http://test/page1"));
        }

        [Test]
        public void Then_one_htmldoc_should_be_returned()
        {
            htmlDocs.Count.Should().Be(1);
        }
    }
}