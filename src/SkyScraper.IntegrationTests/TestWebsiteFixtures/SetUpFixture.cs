using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace SkyScraper.IntegrationTests.TestWebsiteFixtures
{
    [SetUpFixture]
    public class SetUpFixture
    {
        readonly List<Process> processes = new List<Process>();

        [SetUp]
        public void SetUp()
        {
            Process.GetProcesses().Where(x => x.ProcessName.Contains("iisexpress")).ToList().ForEach(x => x.Kill());
            var apiDir = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("src\\") + 4) + "SkyScraper.TestWebsite";
            StartProcess(@"C:\Program Files\IIS Express\iisexpress.exe", @"/path:" + apiDir + " /port:12345");
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch { }
            }
        }

        void StartProcess(string fileName, string arguments = null)
        {
            var processStartInfo = new ProcessStartInfo(fileName, arguments) { WindowStyle = ProcessWindowStyle.Minimized };
            var process = Process.Start(processStartInfo);
            processes.Add(process);
        }
    }
}