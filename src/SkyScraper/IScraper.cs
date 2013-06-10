using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkyScraper
{
    public interface IScraper
    {
        IDisposable Subscribe(IObserver<HtmlDoc> observer);
        Task Scrape(Uri uri);
        List<IObserver<HtmlDoc>> Observers { get; set; }
        TimeSpan TimeOut { set; }
        Regex IgnoreLinks { set; }
        int? MaxDepth { set; }
        Regex IncludeLinks { set; }
        Regex ObserverLinkFilter { set; }
        bool EnableRobotsProtocol { get; set; }
        string UserAgentName { get; set; }
    }
}