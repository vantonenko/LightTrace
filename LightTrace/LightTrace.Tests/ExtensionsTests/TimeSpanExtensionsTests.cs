using System;
using LightTrace.Extensions;
using NUnit.Framework;

namespace LightTrace.Tests.ExtensionsTests;

public class TimeSpanExtensionsTests
{
    [Test]
    [TestCase(0, 0, 0)]
    [TestCase(1, 2, 1)]
    [TestCase(2, 1, 1)]
    public void Min_Success(int aTicks, int bTicks, int expectedTicks) =>
        Assert.That(
            new TimeSpan(aTicks).Min(new TimeSpan(bTicks)), 
            Is.EqualTo(new TimeSpan(expectedTicks)));

    [Test]
    [TestCase(0, 0, 0)]
    [TestCase(1, 2, 2)]
    [TestCase(2, 1, 2)]
    public void Max_Success(int aTicks, int bTicks, int expectedTicks) =>
        Assert.That(
            new TimeSpan(aTicks).Max(new TimeSpan(bTicks)), 
            Is.EqualTo(new TimeSpan(expectedTicks)));
}