using LightTrace.Extensions;

namespace LightTrace.Tests
{
    public class StringExtensionsTests
    {
        [Test]
        [TestCase("", 0, "")]
        [TestCase("", 10, "")]
        [TestCase("123", 10, "123")]
        [TestCase("123", 0, "")]
        [TestCase("123", 2, "12")]
        public void TakeFirst_Empty_Success(string input, int count, string expectedOutput) => 
            Assert.That(input.TakeFirst(count), Is.EqualTo(expectedOutput));
    }
}