using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace SkyScraper.Tests.ImageScraperObserverFixtures
{
    [TestFixture]
    class When_html_contains_a_local_image_and_httpclient_throws_exception : ConcernForImageScraperObserverOnNext
    {
        protected override void Context()
        {
            base.Context();
            HtmlDoc.Html = @"<html>
                         <img src=""image.png"" />
                         </html>";
            HtmlDoc.Uri = new Uri("http://test/");
            HttpClient.GetByteArray(Arg.Any<Uri>()).Returns(new Task<byte[]>(() => { throw new Exception(); }));
        }

        [Test]
        public void Then_file_should_not_be_written()
        {
            FileWriter.DidNotReceive().Write(Arg.Any<string>(), Arg.Any<byte[]>());
        }
    }
}