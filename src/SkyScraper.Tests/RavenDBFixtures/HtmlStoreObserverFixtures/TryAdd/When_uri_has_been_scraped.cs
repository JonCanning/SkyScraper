using FluentAssertions;
using NUnit.Framework;
using System;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures.TryAdd
{
    [TestFixture]
    class When_uri_has_been_scraped : ConcernForHtmlStoreObserver
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
        public void Then_one_htmldoc_should_be_stored()
        {
            DocumentSession.Query<HtmlDoc>().Should().HaveCount(1);
        }

        [Test]
        public void Then_result_should_be_false()
        {
            result.Should().BeFalse();
        }
    }
}