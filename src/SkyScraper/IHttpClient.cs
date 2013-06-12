using System;
using System.IO;
using System.Threading.Tasks;

namespace SkyScraper
{
    public interface IHttpClient
    {
        Task<string> GetString(Uri uri);
        Task<byte[]> GetByteArray(Uri uri);
        string UserAgentName { set; get; }
        Task<Stream> GetStream(Uri uri);
    }
}