using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class HttpClient : IHttpClient
    {
        readonly System.Net.Http.HttpClient httpClient;
        string userAgentName;

        public string UserAgentName
        {
            set
            {
                userAgentName = value;
                const string name = "User-Agent";
                httpClient.DefaultRequestHeaders.Remove(name);
                httpClient.DefaultRequestHeaders.Add(name, value);
            }
            get { return userAgentName; }
        }

        public HttpClient()
        {
            httpClient = new System.Net.Http.HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { Timeout = TimeSpan.FromMinutes(1) };
        }

        public async Task<string> GetString(Uri uri)
        {
            return await Get(uri, x => x.ReadAsStringAsync());
        }

        public async Task<byte[]> GetByteArray(Uri uri)
        {
            return await Get(uri, x => x.ReadAsByteArrayAsync());
        }

        public async Task<Stream> GetStream(Uri uri)
        {
            return await Get(uri, x => x.ReadAsStreamAsync());
        }

        async Task<T> Get<T>(Uri uri, Func<HttpContent, Task<T>> content)
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
            {
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK || httpRequestMessage.RequestUri != uri)
                    return default(T);
                return await content(httpResponseMessage.Content);
            }
        }
    }
}