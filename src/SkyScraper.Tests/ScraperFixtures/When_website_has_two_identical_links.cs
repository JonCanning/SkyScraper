using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_website_has_two_identical_links : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            page = @"<html>
                         <a href=""page1"">link1</a>
                         <a href=""page1"">link1</a>
                         </html>";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_link_should_be_downloaded_once()
        {
            HttpClient.Received(1).GetString(Arg.Is<Uri>(x => x.ToString() == "http://test/page1"));
        }
    }

    [TestFixture]
    class When_robots_protocol_enabled_and_link_is_disallowed : ConcernForScraper
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
            var path = Path.Combine(Environment.CurrentDirectory, "ScraperFixtures\\robots.txt");
            var robots = File.OpenText(path).ReadToEnd();
            HttpClient.GetString(new Uri("http://test/robots.txt")).Returns(Task.Factory.StartNew(() => robots));
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        protected override void Because()
        {
            SUT.EnableRobotsProtocol = true;
            base.Because();
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page()
        {
            htmlDocs.Should().Contain(x => x.Uri.ToString() == "http://test/" && x.Html == page);
        }

        [Test]
        public void Then_htmldocs_should_not_contain_first_page()
        {
            htmlDocs.Should().NotContain(x => x.Uri.ToString() == "http://test/page1" && x.Html == "/page1");
        }
    }
}