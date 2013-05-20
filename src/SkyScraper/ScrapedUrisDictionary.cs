using System;
using System.Collections.Concurrent;

namespace SkyScraper
{
    public class ScrapedUrisDictionary : IScrapedUris
    {
        readonly ConcurrentDictionary<string, string> scrapedHtmlDocs = new ConcurrentDictionary<string, string>();

        public bool TryAdd(Uri uri)
        {
            return scrapedHtmlDocs.TryAdd(uri.PathAndQuery, null);
        }
    }
}