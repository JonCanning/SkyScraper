SkyScraper
==========

An asynchronous web scraper / web crawler using async / await and  Reactive Extensions 

Usage
- 
    var scraper = new Scraper(new HttpClient(), new ScrapedUrisDictionary()); //use built in IHttpClient and IScrapedUris implementations
    var io = new ImageScraperObserver(new HttpClient(), new FileWriter(new DirectoryInfo("c:\\temp")));
    scraper.Subscribe(io); //use built in image scraper
    scraper.Subscribe(new ConsoleWriterObserver()); //use built in console writer
    scraper.Subscribe(x => Console.WriteLine(x.Uri)); //implement your own subscriber
    scraper.MaxDepth = 2; //optional
    scraper.TimeOut = TimeSpan.FromMinutes(5); //optional
    scraper.IgnoreLinks = new Regex("spam"); //Ignore links in page
    scraper.IncludeLinks = new Regex("stuff"); //Scrape links on page
    scraper.ObserverLinkFilter = new Regex("things"); //Trigger observers when link matches
    await scraper.Scrape(new Uri("http://www.mywebsite.com/"));
