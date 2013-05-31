using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_website_contains_a_link_longer_than_2048_characters : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;
        string link;

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            link = Enumerable.Repeat("a", 2048).Aggregate(string.Empty, (s, s1) => s += s1);
            page = @"<html>
                         <a>link1</a>
                         <a href=""{0}"">link1</a>
                         </html>";
            page = string.Format(page, link);
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_link_should_not_be_followed()
        {
            HttpClient.DidNotReceive().GetString(Arg.Is<Uri>(x => x.ToString().EndsWith(link)));
        }

        [Test]
        public void Then_one_htmldoc_should_be_returned()
        {
            htmlDocs.Count.Should().Be(1);
        }
    }
}