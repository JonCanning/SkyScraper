using System.IO;

namespace SkyScraper.Observers.ImageScraper
{
    public class FileWriter : IFileWriter
    {
        readonly DirectoryInfo directoryInfo;

        public FileWriter(DirectoryInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo;
        }

        public void Write(string fileName, byte[] bytes)
        {
            fileName = Path.Combine(directoryInfo.FullName, fileName);
            using (var fileStream = File.OpenWrite(fileName))
                fileStream.Write(bytes, 0, bytes.Length);
        }
    }
}