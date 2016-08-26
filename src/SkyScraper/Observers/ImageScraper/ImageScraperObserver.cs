using CsQuery;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SkyScraper.Observers.ImageScraper
{
    public class ImageScraperObserver : IObserver<HtmlDoc>
    {
        public event Action<Exception> OnOperationCanceledException = delegate { };
        
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
            downloadUris.AsParallel().ForAll(DownloadImage);
        }

        public void OnError(Exception error) { }

        public void OnCompleted() { }

        async void DownloadImage(Uri uri)
        {
            var fileName = uri.Segments.Last();
            if (!downloadedImages.TryAdd(fileName, null))
                return;
            try
            {
                var imgBytes = await httpClient.GetByteArray(uri);
                fileWriter.Write(fileName, imgBytes);
            }
            catch (OperationCanceledException ex)
            {
                OnOperationCanceledException(ex);
            }
        }
    }
}
