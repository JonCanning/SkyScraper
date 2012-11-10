using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SkyScraper.Tests.ImageScraperObserverFixtures
{
    [TestFixture]
    class When_html_contains_two_identical_images : ConcernForImageScraperObserverOnNext
    {
        protected override void Context()
        {
            base.Context();
            HtmlDoc.Html = @"<html>
                         <img src=""image.png"" />
                         <img src=""image.png"" />
                         </html>";
            HtmlDoc.Uri = new Uri("http://test/");
            HttpClient.GetByteArray(Arg.Any<Uri>()).Returns(new Task<byte[]>(() => new byte[0]));
        }

        [Test]
        public void Then_http_client_should_download_image_once()
        {
            HttpClient.Received(1).GetByteArray(Arg.Is<Uri>(x => x.ToString() == "http://test/image.png"));
        }
    }
}