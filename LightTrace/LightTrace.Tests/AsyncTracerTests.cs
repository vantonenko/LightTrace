using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LightTrace.Tests;

[NonParallelizable]
internal class AsyncTracerTests
{
    private const int TestDelayMilliseconds = 100;

    [SetUp]
    public void SetupBeforeEachTest()
    {
        CallStackTrace.Reset();
    }

    [Test]
    [TestCase(1, 1, new[] {"Test"})]
    [TestCase(20, 1, new[] { "Test" })]
    [TestCase(20, 2, new[] { "Test" })]
    [TestCase(20, 10, new[] { "Test" })]
    [TestCase(1, 1, new[] { "Test", "Test1" })]
    [TestCase(20, 1, new[] { "Test", "Test1" })]
    [TestCase(20, 2, new[] { "Test", "Test1" })]
    [TestCase(20, 10, new[] { "Test", "Test1" })]
    [TestCase(1, 1, new[] { "Test", "Test1", "Test2" })]
    [TestCase(20, 1, new[] { "Test", "Test1", "Test2" })]
    [TestCase(20, 2, new[] { "Test", "Test1", "Test2" })]
    [TestCase(20, 10, new[] { "Test", "Test1", "Test2" })]
    public void GetCalls_Success(int deep, int threadCount, string[] testNames)
    {
        var tasks = new Task[threadCount];
        foreach (string testName in testNames)
        {
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = CallFunctionAsync(testName, deep);
            }
        }
        Task.WaitAll(tasks);

        CallContext callContext = CallStackTrace.GetCalls();

        foreach (string testName in testNames)
        {
            ValidateResult(callContext.Calls.Single(call => call.Name == testName), testName, deep);
        }
    }

    [Test]
    public void AsMdReportInMillisecond_NoFilter()
    {
        using (new CallStackTrace())
        {
            for (int i = 0; i < 10; i++)
            {
                CallLevel1();
            }
        }
        CallContext callContext = CallStackTrace.GetCalls();
        var mdLines = callContext.AsMdReportInMillisecond(0).ToList();
        Assert.That(mdLines.Count, Is.EqualTo(6));
    }

    [Test]
    public void AsMdReportInMillisecond_WithFilter()
    {
        using (new CallStackTrace())
        {
            for (int i = 0; i < 10; i++)
            {
                CallLevel1();
            }
        }
        CallContext callContext = CallStackTrace.GetCalls();
        var mdLines = callContext.AsMdReportInMillisecond(1500).ToList();
        Assert.That(mdLines.Count, Is.EqualTo(5));
    }

    private async Task CallFunctionAsync(string funName, int callDeep, int currentDeep = 0)
    {
        using (new CallStackTrace(funName))
        {
            await Task.Delay(TestDelayMilliseconds);
            currentDeep++;
            if (callDeep > currentDeep)
            {
                await CallFunctionAsync(GenerateFuncName(funName, currentDeep), callDeep, currentDeep);
            }
        }
    }

    private string GenerateFuncName(string funcName, int deep)
        => $"{funcName}_{deep}";

    private void ValidateResult(CallContext context, string funcName, int deep, int currentDeep = 0, long parentTotalTicks = long.MaxValue)
    {
        currentDeep++;
        if (deep > currentDeep)
        {
            var fName = GenerateFuncName(funcName, currentDeep);
            var currentContext = context.Calls.Single(call => call.Name == fName);
            ValidateResult(currentContext, fName, deep, currentDeep, currentContext.TotalTicks);
        }
        else
        {
            Assert.IsEmpty(context.Calls);
        }

        Assert.That(context.Name, Is.EqualTo(funcName));
        Assert.LessOrEqual(context.TotalTicks, parentTotalTicks);
    }

    private void CallLevel1()
    {
        using (new CallStackTrace())
        {
            Thread.Sleep(100);
            CallLevel2();
        }
    }

    private void CallLevel2()
    {
        using (new CallStackTrace())
        {
            Thread.Sleep(100);
        }
    }

}