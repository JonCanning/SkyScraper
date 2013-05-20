using System;
using System.Collections.Concurrent;

namespace SkyScraper
{
    public class ScrapedDocumentsDictionary : IScrapedDocuments
    {
        readonly ConcurrentDictionary<string, string> scrapedHtmlDocs = new ConcurrentDictionary<string, string>();

        public bool TryAdd(Uri uri)
        {
            return scrapedHtmlDocs.TryAdd(uri.PathAndQuery, null);
        }
    }
}