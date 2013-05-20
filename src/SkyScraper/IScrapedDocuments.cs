using System;

namespace SkyScraper
{
    public interface IScrapedDocuments
    {
        bool TryAdd(Uri uri);
    }
}