using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            HttpClient.GetString(Uri).Returns(new Task<string>(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(
                x => new Task<string>(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_link_should_be_downloaded_once()
        {
            HttpClient.Received(1).GetString(Arg.Is<Uri>(x => x.ToString() == "http://test/page1"));
        }
    }
}