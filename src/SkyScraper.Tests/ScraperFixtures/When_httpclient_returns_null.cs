using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_httpclient_returns_null : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();
        string page;

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            page = null;
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_no_htmldoc_should_be_returned()
        {
            htmlDocs.Should().BeEmpty();
        }
    }
}