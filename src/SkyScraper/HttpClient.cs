using System;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class HttpClient : IHttpClient
    {
        readonly System.Net.Http.HttpClient httpClient;

        public HttpClient()
        {
            httpClient = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromMinutes(1) };
        }

        public Task<string> GetString(Uri uri)
        {
            return httpClient.GetStringAsync(uri);
        }

        public Task<byte[]> GetByteArray(Uri uri)
        {
            return httpClient.GetByteArrayAsync(uri);
        }
    }
}