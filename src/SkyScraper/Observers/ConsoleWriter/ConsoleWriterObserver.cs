using System;

namespace SkyScraper.Observers.ConsoleWriter
{
    public class ConsoleWriterObserver : IObserver<HtmlDoc>
    {
        public void OnNext(HtmlDoc htmlDoc)
        {
            Console.WriteLine(htmlDoc.Uri.ToString());
        }

        public void OnError(Exception error) {}

        public void OnCompleted() {}
    }
}