using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;

namespace SkyScraper.Tests.RobotsFixtures
{
    [TestFixture]
    class When_loading_a_robots_txt_as_a_named_user_agent
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "RobotsFixtures\\robots.txt");
            var robots = File.OpenText(path).ReadToEnd();
            Robots.Load(robots, "bot");
        }

        [Test]
        public void Then_path1_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/path1").Should().BeFalse();
        }

        [Test]
        public void Then_path2_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/path2").Should().BeFalse();
        }

        [Test]
        public void Then_botpath1_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/botpath1").Should().BeFalse();
        }

        [Test]
        public void Then_path3_filetxt_should_be_allowed()
        {
            Robots.PathIsAllowed("/foo/path3/file.txt").Should().BeTrue();
        }
    }
}