using LightTrace.Extensions;
using NUnit.Framework;

namespace LightTrace.Tests.ExtensionsTests
{
    public class StringExtensionsTests
    {
        [Test]
        [TestCase("", 0, "")]
        [TestCase("", 10, "")]
        [TestCase("123", 10, "123")]
        [TestCase("123", 0, "")]
        [TestCase("123", 2, "12")]
        public void TakeFirst_Success(string input, int count, string expected) =>
            Assert.That(input.TakeFirst(count), Is.EqualTo(expected));
    }
}