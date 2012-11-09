using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_website_contains_an_explicit_local_link : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            page = @"<html>
                         <a href=""http://test/page1"">link1</a>
                         </html>";
            HttpClient.GetString(Uri).Returns(new Task<string>(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => new Task<string>(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_two_htmldocs_should_be_returned()
        {
            htmlDocs.Count.Should().Be(2);
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page()
        {
            htmlDocs.Should().Contain(x => x.Uri.ToString() == "http://test/" && x.Html == page);
        }

        [Test]
        public void Then_htmldocs_should_first_page()
        {
            htmlDocs.Should().Contain(x => x.Uri.ToString() == "http://test/page1" && x.Html == "/page1");
        }
    }
}