using System;

namespace SkyScraper
{
    public interface IScrapedUris
    {
        bool TryAdd(Uri uri);
    }
}