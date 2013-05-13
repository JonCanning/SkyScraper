using System.Threading.Tasks;

namespace SkyScraper.Observers.ImageScraper
{
    public interface IFileWriter
    {
        Task Write(string fileName, byte[] bytes);
    }
}