using FluentAssertions;
using NUnit.Framework;
using Raven.Client;
using System;
using System.Linq;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures.OnNext
{
    [TestFixture]
    class When_an_html_doc_is_received : ConcernForHtmlStoreObserver
    {
        IDocumentSession documentSession;

        protected override void Because()
        {
            var htmlDoc = new HtmlDoc { Uri = new Uri("http://test/") };
            documentSession = DocumentSessionFactory.CreateNewStore();
            documentSession.Store(htmlDoc);
            documentSession.SaveChanges();
            htmlDoc.Html = "html";
            SUT.OnNext(htmlDoc);
        }

        [Test]
        public void Then_the_html_doc_should_be_stored()
        {
            documentSession.Query<HtmlDoc>().Single().Html.Should().Be("html");
        }
    }
}