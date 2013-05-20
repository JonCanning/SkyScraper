using FluentAssertions;
using NUnit.Framework;
using Raven.Client;
using System;
using System.Linq;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures.TryAdd
{
    [TestFixture]
    class When_uri_has_not_been_scraped : ConcernForHtmlStoreObserver
    {
        bool result;
        IDocumentSession documentSession;

        protected override void Because()
        {
            documentSession = DocumentSessionFactory.CreateNewStore();
            result = SUT.TryAdd(new Uri("http://test/"));
        }

        [Test]
        public void Then_result_should_be_true()
        {
            result.Should().BeTrue();
        }

        [Test]
        public void Then_uri_should_be_saved()
        {
            documentSession.Query<HtmlDoc>().Single().Uri.Should().Be("http://test/");
        }
    }
}