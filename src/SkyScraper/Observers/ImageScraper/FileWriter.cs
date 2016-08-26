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
            if (fileName != null | bytes != null)
            {
                fileName = Path.Combine(directoryInfo.FullName, fileName);
                using (var fileStream = File.OpenWrite(fileName))
                {
                    if (fileStream != null)
                        fileStream.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
