using ConsoleApp2.Tracing;
using ConsoleApp2.Tracing.Data;
using ConsoleApp2.Tracing.Extensions;

using (new Tracer("Trace1"))
{
    foreach (var _ in Enumerable.Range(0, 2))
    {
        using (new Tracer("Trace11"))
        {
            using (new Tracer("Trace111"))
            {
                Thread.Sleep(50);
            }

            foreach (var __ in Enumerable.Range(0, 3))
            {
                using (new Tracer("Trace112"))
                {
                    Thread.Sleep(50);
                }
            }
        }
    }

    Thread.Sleep(50);

    using (new Tracer("Trace12"))
    {
        Thread.Sleep(50);
    }
}

using (new Tracer("Trace2"))
{
    Thread.Sleep(50);
}

foreach (TraceEntries traceEntries in Tracer.GetRootTraceEntries())
{
    Console.WriteLine($"Traces:\n{traceEntries.AsMdReport()}");
}