using System.Threading.Tasks;

namespace SkyScraper.Observers.ImageScraper
{
    public interface IFileWriter
    {
        void Write(string fileName, byte[] bytes);
    }
}