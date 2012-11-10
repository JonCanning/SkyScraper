using System;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ImageScraperObserverFixtures
{
    [TestFixture]
    class When_html_contains_an_external_image : ConcernForImageScraperObserverOnNext
    {
        protected override void Context()
        {
            base.Context();
            HtmlDoc.Html = @"<html>
                         <img src=""http://foo.image.png"" />
                         </html>";
            HtmlDoc.Uri = new Uri("http://test/");
        }

        [Test]
        public void Then_http_client_should_not_download_image()
        {
            HttpClient.DidNotReceive().GetByteArray(Arg.Any<Uri>());
        }
    }
}