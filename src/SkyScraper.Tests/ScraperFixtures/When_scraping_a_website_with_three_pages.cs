using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_scraping_a_website_with_three_pages : ConcernForScraper
    {
        readonly List<string> urls = new List<string>();

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            var page = @"<html>
                         <a href=""page1"">link1</a>
                         <a href=""page2"">link2</a>
                         </html>";
            HttpClient.GetString(Uri).Returns(new Task<string>(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => new Task<string>(() => ""));
            OnNext = x => urls.Add(x.Uri.ToString());
        }

        [Test]
        public void Then_()
        {
            urls.Should().BeEquivalentTo(new[] { "http://test/", "http://test/page1", "http://test/page2" });
        }
    }
}
