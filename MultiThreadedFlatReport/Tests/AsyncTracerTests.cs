namespace LightTrace.Tests;

internal class AsyncTracerTests
{
    private const int TestDelayMilliseconds = 100;

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
    public void Calls_Success(int deep, int threadCount, string[] testNames)
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

        foreach (string testName in testNames)
        {
            ValidateResult(AsyncTracer.Calls[testName], testName, deep);
        }
    }

    private async Task CallFunctionAsync(string funName, int callDeep, int currentDeep = 0)
    {
        using (new AsyncTracer(funName))
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

    private void ValidateResult(AsyncTracer.AsyncContext context, string funcName, int deep, int currentDeep = 0)
    {
        currentDeep++;
        if (deep > currentDeep)
        {
            var fName = GenerateFuncName(funcName, currentDeep);
            ValidateResult(context.Calls[fName], fName, deep, currentDeep);
        }

        Assert.That(context.Name, Is.EqualTo(funcName));
        if (context.Parent != null)
        {
            Assert.LessOrEqual(context.TotalTicks, context.Parent.TotalTicks);
        }
    }

}