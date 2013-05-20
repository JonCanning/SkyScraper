using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace SkyScraper.Tests.RavenDBFixtures.ScrapedUrisFixtures
{
    [TestFixture]
    class When_uri_has_not_been_scraped : ConcernForIScrapedUris
    {
        bool result;

        protected override void Because()
        {
            result = SUT.TryAdd(new Uri("http://test/"));
            DocumentSession.SaveChanges();
        }


        [Test]
        public void Then_result_should_be_true()
        {
            result.Should().BeTrue();
        }

        [Test]
        public void Then_uri_should_be_saved()
        {
            DocumentSession.Query<HtmlDoc>().Single().Uri.Should().Be("http://test/");
        }
    }
}