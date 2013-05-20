using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures.TryAdd
{
    [TestFixture]
    class When_uri_has_not_been_scraped : ConcernForHtmlStoreObserver
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