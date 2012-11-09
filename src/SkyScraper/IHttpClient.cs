using System;
using System.Threading.Tasks;

namespace SkyScraper
{
    public interface IHttpClient
    {
        Task<string> GetString(Uri uri);
    }
}