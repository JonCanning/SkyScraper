using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace SkyScraper.Tests.RavenDBFixtures.HtmlStoreObserverFixtures
{
    [TestFixture]
    class When_an_html_doc_is_received : ConcernForHtmlStoreObserver
    {
        protected override void Because()
        {
            var htmlDoc = new HtmlDoc { Uri = new Uri("http://test/"), Html = "html" };
            DocumentSession.Store(htmlDoc);
            DocumentSession.SaveChanges();
            SUT.OnNext(htmlDoc);
        }

        [Test]
        public void Then_the_html_doc_should_be_stored()
        {
            DocumentSession.Query<HtmlDoc>().Single().Html.Should().Be("html");
        }
    }
}