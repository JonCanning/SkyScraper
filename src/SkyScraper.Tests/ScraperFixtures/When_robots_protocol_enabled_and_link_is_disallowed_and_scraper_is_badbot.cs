using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_robots_protocol_enabled_and_link_is_disallowed_and_scraper_is_badbot : ConcernForScraper
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
            HttpClient.UserAgentName = "badbot";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            HttpClient.GetString(Arg.Is<Uri>(x => x == new Uri("http://test/robots.txt"))).Returns(Task.Factory.StartNew(() => robots));
            OnNext = x => htmlDocs.Add(x);
        }

        protected override void Because()
        {
            SUT.DisableRobotsProtocol = false;
            base.Because();
        }

        [Test]
        public void Then_htmldocs_should_not_contain_home_page()
        {
            htmlDocs.Should().NotContain(x => x.Uri.ToString() == "http://test/" && x.Html == page);
        }

        [Test]
        public void Then_htmldocs_should_not_contain_first_page()
        {
            htmlDocs.Should().NotContain(x => x.Uri.ToString() == "http://test/page1" && x.Html == "/page1");
        }

        [Test]
        public void Then_useragent_should_be_set_on_httpclient()
        {
            HttpClient.Received().UserAgentName = "badbot";
        }
    }
}