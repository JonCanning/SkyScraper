using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyScraper
{
    public interface IScraper {
        IDisposable Subscribe(IObserver<HtmlDoc> observer);
        Task Scrape(Uri uri);
        List<IObserver<HtmlDoc>> Observers { get; set; }
        TimeSpan TimeOut { set; }
    }
}