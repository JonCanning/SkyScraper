using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SkyScraper.Observers.ImageScraper
{
    public class ImageScraperObserver : IObserver<HtmlDoc>
    {
        readonly ConcurrentDictionary<string, string> downloadedImages = new ConcurrentDictionary<string, string>();
        readonly IFileWriter fileWriter;
        readonly IHttpClient httpClient;

        public ImageScraperObserver(IHttpClient httpClient, IFileWriter fileWriter)
        {
            this.httpClient = httpClient;
            this.fileWriter = fileWriter;
        }

        public async void OnNext(HtmlDoc htmlDoc)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlDoc.Html);
            var linkNodeCollection = htmlDocument.DocumentNode.SelectNodes("//img[@src]");
            if (linkNodeCollection == null || !linkNodeCollection.Any())
                return;
            var baseUri = new Uri(htmlDoc.Uri.GetLeftPart(UriPartial.Path));
            if (baseUri.Segments.Last().Contains('.'))
                baseUri = new Uri(baseUri.ToString().Substring(0, baseUri.ToString().LastIndexOf('/')));
            var imgSrcs = linkNodeCollection.Select(x => x.Attributes["src"].Value).Where(x => x.LinkIsLocal(baseUri.ToString()));
            foreach (var downloadUri in imgSrcs.Select(imgSrc => Uri.IsWellFormedUriString(imgSrc, UriKind.Absolute) ? new Uri(imgSrc) : new Uri(baseUri, imgSrc)))
                await DownloadImage(downloadUri);
        }

        public void OnError(Exception error) {}

        public void OnCompleted() {}

        async Task DownloadImage(Uri uri)
        {
            var fileName = uri.Segments.Last();
            if (!downloadedImages.TryAdd(fileName, null))
                return;
            var imgBytes = await httpClient.GetByteArray(uri);
            await fileWriter.Write(fileName, imgBytes);
        }
    }
}