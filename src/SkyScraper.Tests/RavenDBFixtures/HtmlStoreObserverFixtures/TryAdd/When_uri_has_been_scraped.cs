using FluentAssertions;
using NUnit.Framework;
using Raven.Client;
using System;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures.TryAdd
{
    [TestFixture]
    class When_uri_has_been_scraped : ConcernForHtmlStoreObserver
    {
        bool result;
        IDocumentSession documentSession;

        protected override void Because()
        {
            documentSession = DocumentSessionFactory.CreateNewStore();
            documentSession.Store(new HtmlDoc { Uri = new Uri("http://test/") });
            documentSession.SaveChanges();
            result = SUT.TryAdd(new Uri("http://test/"));
            documentSession.SaveChanges();
        }

        [Test]
        public void Then_one_htmldoc_should_be_stored()
        {
            documentSession.Query<HtmlDoc>().Should().HaveCount(1);
        }

        [Test]
        public void Then_result_should_be_false()
        {
            result.Should().BeFalse();
        }
    }
}