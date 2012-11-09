using NUnit.Framework;

namespace SkyScraper.Tests
{
    abstract class ConcernFor<T> where T : class
    {
        protected T SUT;

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            Context();
            SUT = CreateClassUnderTest();
            Because();
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown() { }

        protected virtual void Context() { }
        protected abstract T CreateClassUnderTest();
        protected virtual void Because() { }
    }
}