using System;
using FluentAssertions;
using NUnit.Framework;

namespace SkyScraper.Tests.RavenDBFixtures.ScrapedUrisFixtures
{
    [TestFixture]
    class When_uri_has_been_scraped : ConcernForIScrapedUris
    {
        bool result;

        protected override void Because()
        {
            DocumentSession.Store(new HtmlDoc { Uri = new Uri("http://test/") });
            DocumentSession.SaveChanges();
            result = SUT.TryAdd(new Uri("http://test/"));
            DocumentSession.SaveChanges();
        }


        [Test]
        public void Then_result_should_be_false()
        {
            result.Should().BeFalse();
        }

        [Test]
        public void Then_one_htmldoc_should_be_stored()
        {
            DocumentSession.Query<HtmlDoc>().Should().HaveCount(1);
        }
    }
}