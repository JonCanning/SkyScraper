using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_scraping_a_subdirectory : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test/foo");
            page = @"<html>
                         <a href=""page1"">link1</a>
                         </html>";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page()
        {
            htmlDocs.Should().Contain(x => x.Uri.ToString() == "http://test/foo" && x.Html == page);
        }

        [Test]
        public void Then_htmldocs_should_first_page()
        {
            htmlDocs.Should().Contain(x => x.Uri.ToString() == "http://test/foo/page1" && x.Html == "/foo/page1");
        }

        [Test]
        public void Then_two_htmldocs_should_be_returned()
        {
            htmlDocs.Count.Should().Be(2);
        }
    }
}