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

        public async Task<string> GetString(Uri uri)
        {
            return await httpClient.GetStringAsync(uri);
        }

        public async Task<byte[]> GetByteArray(Uri uri)
        {
            return await httpClient.GetByteArrayAsync(uri);
        }
    }
}