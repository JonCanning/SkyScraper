using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SkyScraper.Observers.ImageScraper
{
    public class ImageScraperObserver : IObserver<HtmlDoc>, IDisposable
    {
        readonly ConcurrentDictionary<string, string> downloadedImages;
        readonly ITaskRunner taskRunner;
        readonly IHttpClient httpClient;
        readonly IFileWriter fileWriter;

        public ImageScraperObserver(IFileWriter fileWriter)
        {
            this.fileWriter = fileWriter;
            taskRunner = taskRunner ?? new AsyncTaskRunner();
            httpClient = httpClient ?? new AsyncHttpClient();
            downloadedImages = new ConcurrentDictionary<string, string>();
        }

        public ImageScraperObserver(ITaskRunner taskRunner, IHttpClient httpClient, IFileWriter fileWriter)
            : this(fileWriter)
        {
            this.taskRunner = taskRunner;
            this.httpClient = httpClient;
            this.fileWriter = fileWriter;
        }


        public void OnNext(HtmlDoc htmlDoc)
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
            foreach (var downloadUri in imgSrcs.Select(imgSrc => Uri.IsWellFormedUriString(imgSrc, UriKind.Absolute) ? imgSrc : baseUri + imgSrc))
            {
                taskRunner.Run(() => DownloadImage(new Uri(downloadUri)));
            }
        }

        void DownloadImage(Uri uri)
        {
            var fileName = uri.AbsolutePath.TrimStart('/');
            if (downloadedImages.ContainsKey(fileName))
                return;
            downloadedImages.TryAdd(fileName, null);
            var task = httpClient.GetByteArray(uri);
            taskRunner.Run(task);
            byte[] imgBytes;
            try
            {
                imgBytes = task.Result;
            }
            catch
            {
                return;
            }
            taskRunner.Run(() => fileWriter.Write(fileName, imgBytes));
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        public void Dispose()
        {
            taskRunner.WaitForAllTasks();
        }
    }
}