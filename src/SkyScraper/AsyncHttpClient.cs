using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class AsyncHttpClient : IHttpClient
    {
        readonly HttpClient httpClient;

        public AsyncHttpClient()
        {
            httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };
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