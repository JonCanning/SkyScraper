using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_website_contains_link_to_path_not_in_baseuri : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test/foo/page1");
            var page = @"<html>
                         <a href=""/bar"">link1</a>
                         </html>";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_only_baseuri_should_be_downloaded()
        {
            htmlDocs.Should().HaveCount(1).And.Contain(x => x.Uri.ToString() == "http://test/foo/page1");
        }
    }
}