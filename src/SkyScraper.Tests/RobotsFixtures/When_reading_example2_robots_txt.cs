using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace SkyScraper.Tests.RobotsFixtures
{
    [TestFixture]
    class When_reading_example2_robots_txt
    {
        [Test]
        public void Then_disallows_should_be_respected()
        {
            const string robotsTxt = "RobotsFixtures\\example2.txt";
            Robots.Load(File.ReadAllText(robotsTxt));
            var lines = new Queue<string>(File.ReadAllLines(robotsTxt));
            while (lines.Peek() != "User-agent: *")
            {
                lines.Dequeue();
            }
            lines.Dequeue();
            while (!lines.Peek().StartsWith("#"))
            {
                var line = lines.Dequeue();
                if (string.IsNullOrEmpty(line))
                    continue;
                var rule = line.Split(' ')[1].Replace("*", "foo");
                Robots.PathIsAllowed(rule).Should().BeFalse();
            }

            Robots.PathIsAllowed("/path").Should().BeTrue();
        }
    }
}