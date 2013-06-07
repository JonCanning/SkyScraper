using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using CsQuery;

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

        public void OnNext(HtmlDoc htmlDoc)
        {
            var baseUri = new Uri(htmlDoc.Uri.GetLeftPart(UriPartial.Path));
            if (baseUri.Segments.Last().Contains('.'))
                baseUri = new Uri(baseUri.ToString().Substring(0, baseUri.ToString().LastIndexOf('/')));
            CQ html = htmlDoc.Html;
            var imgSrcs = html["img"].Select(x => x.GetAttribute("src")).Where(x => x.LinkIsLocal(baseUri.ToString()));
            var downloadUris = imgSrcs.Select(imgSrc => Uri.IsWellFormedUriString(imgSrc, UriKind.Absolute) ? new Uri(imgSrc) : new Uri(baseUri, imgSrc));
            downloadUris.AsParallel().ForAll(x => DownloadImage(x));
        }

        public void OnError(Exception error) { }

        public void OnCompleted() { }

        async Task DownloadImage(Uri uri)
        {
            var fileName = uri.Segments.Last();
            if (!downloadedImages.TryAdd(fileName, null))
                return;
            var imgBytes = await httpClient.GetByteArray(uri);
            fileWriter.Write(fileName, imgBytes);
        }
    }
}