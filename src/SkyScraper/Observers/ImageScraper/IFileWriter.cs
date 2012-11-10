namespace SkyScraper.Observers.ImageScraper
{
    public interface IFileWriter
    {
        void Write(string fileName, byte[] bytes);
    }
}