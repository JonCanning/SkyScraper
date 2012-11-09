using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class AsyncHttpClient : IHttpClient
    {
        public Task<string> GetString(Uri uri)
        {
            var httpClient = new HttpClient();
            return httpClient.GetStringAsync(uri);
        }
    }
}