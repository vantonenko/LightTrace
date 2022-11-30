using LightTrace.Extensions;

namespace LightTrace.Tests.ExtensionsTests;

public class UnitsExtensionsTests
{
    [Test]
    [TestCase(TimeSpan.TicksPerMillisecond/2, "0.5 ms")]
    [TestCase(TimeSpan.TicksPerMillisecond + TimeSpan.TicksPerMillisecond/2, "1.5 ms")]
    [TestCase(TimeSpan.TicksPerSecond/2, "500.0 ms")]
    [TestCase(TimeSpan.TicksPerSecond + TimeSpan.TicksPerSecond/2, "1.5 sec")]
    [TestCase(TimeSpan.TicksPerMinute / 2, "30.0 sec")]
    [TestCase(TimeSpan.TicksPerMinute + TimeSpan.TicksPerMinute / 2, "1.5 min")]
    [TestCase(TimeSpan.TicksPerHour / 2, "30.0 min")]
    [TestCase(TimeSpan.TicksPerHour + TimeSpan.TicksPerHour / 2, "1.5 hours")]
    //[TestCase(TimeSpan.TicksPerHour * 100, "100.0 hours")]
    public void AsTime_TimeSpan_Success(long ticks, string expected) => 
        Assert.That(new TimeSpan(ticks).AsTime(), Is.EqualTo(expected));
    
    [Test]
    [TestCase(0.5, "0.5 ms")]
    [TestCase(1.5, "1.5 ms")]
    [TestCase(500.0, "500.0 ms")]
    [TestCase(1500, "1.5 sec")]
    [TestCase(30_000.0, "30.0 sec")]
    [TestCase(90_000.0, "1.5 min")]
    [TestCase(1_800_000.0, "30.0 min")]
    [TestCase(5_400_000.0, "1.5 hours")]
    //[TestCase(TimeSpan.TicksPerHour * 100, "100.0 hours")]
    public void AsTime_double_Success(double milliseconds, string expected) => 
        Assert.That(milliseconds.AsTime(), Is.EqualTo(expected));
}