using System.IO;
using System.Threading.Tasks;

namespace SkyScraper.Observers.ImageScraper
{
    public class FileWriter : IFileWriter
    {
        readonly DirectoryInfo directoryInfo;

        public FileWriter(DirectoryInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo;
        }

        public Task Write(string fileName, byte[] bytes)
        {
            fileName = Path.Combine(directoryInfo.FullName, fileName);
            return Task.Factory.StartNew(() => WriteFile(fileName, bytes));
        }

        static void WriteFile(string fileName, byte[] bytes)
        {
            using (var fileStream = File.OpenWrite(fileName))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}