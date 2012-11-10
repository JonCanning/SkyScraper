using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ImageScrapingObserverFixtures
{
    [TestFixture]
    class When_html_contains_an_explicit_local_image : ConcernForImageScrapingObserverOnNext
    {
        protected override void Context()
        {
            base.Context();
            HtmlDoc.Html = @"<html>
                         <img src=""http://test/image.png"" />
                         </html>";
            HtmlDoc.Uri = new Uri("http://test/");
            HttpClient.GetByteArray(Arg.Any<Uri>()).Returns(new Task<byte[]>(() => new byte[0]));
        }

        [Test]
        public void Then_http_client_should_download_image()
        {
            HttpClient.Received().GetByteArray(Arg.Is<Uri>(x => x.ToString() == "http://test/image.png"));
        }
    }
}