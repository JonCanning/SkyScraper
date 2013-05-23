using System;
using System.Net;
using System.Net.Http;
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

        public async Task<string> GetString(Uri uri)
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
            {
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK || httpRequestMessage.RequestUri != uri)
                    return null;
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }

        public async Task<byte[]> GetByteArray(Uri uri)
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
            {
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                    return null;
                return await httpResponseMessage.Content.ReadAsByteArrayAsync();
            }
        }
    }
}