using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace SkyScraper.Tests.RobotsFixtures
{
    [TestFixture]
    class When_loading_a_robots_txt_as_a_badbot
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "RobotsFixtures\\robots.txt");
            var robots = File.OpenText(path).ReadToEnd();
            Robots.Load(robots, "badbot");
        }

        [Test]
        public void Then_path1_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/path1").Should().BeFalse();
        }

        [Test]
        public void Then_path1_foo_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/path1/foo").Should().BeFalse();
        }

        [Test]
        public void Then_path2_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/path2").Should().BeFalse();
        }

        [Test]
        public void Then_botpath_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/botpath").Should().BeFalse();
        }

        [Test]
        public void Then_foo_path3_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/foo/path3").Should().BeFalse();
        }

        [Test]
        public void Then_path1_filetxt_should_not_be_allowed()
        {
            Robots.PathIsAllowed("/path1/file.txt").Should().BeFalse();
        }
    }
}