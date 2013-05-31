SkyScraper
==========

An asynchronous web scraper / web crawler using async / await and  Reactive Extensions 

Usage
- 
    var scraper = new Scraper(new HttpClient(), new ScrapedUrisDictionary());
    var io = new ImageScraperObserver(new HttpClient(), new FileWriter(new DirectoryInfo("c:\\temp")));
    scraper.Subscribe(io);
    scraper.Subscribe(new ConsoleWriterObserver());
    scraper.Subscribe(x => Console.WriteLine(x.Uri));
    scraper.MaxDepth = 2; //optional
    scraper.TimeOut = TimeSpan.FromMinutes(5); //optional
    scraper.IgnoreLinks = new Regex("spam"); //optional
    await scraper.Scrape(new Uri("http://www.cambridgecupcakes.com/"));
