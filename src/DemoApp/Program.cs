using LightTrace;
using LightTrace.Extensions;
using static LightTrace.ShortCuts;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedVariable

Console.WriteLine("Press any key to stop...");
Console.WriteLine($"Check '{TraceReport.ReportFile}' file for the generated traces...");

var rnd = new Random();
var cursorTop = Console.CursorTop;
do
{
    Parallel.ForEach(Enumerable.Range(0, 4), _0 =>
    {
        using (new Tracer("Trace1"))
        {
            foreach (var _ in Enumerable.Range(0, 2))
            {
                using (new Tracer("Trace11"))
                {
                    using (new Tracer("Trace111"))
                    {
                        DoWork(0);
                    }

                    foreach (var __ in Enumerable.Range(0, 3))
                    {
                        using (new Tracer("Trace112"))
                        {
                            DoWork(0);
                        }
                    }
                }
            }

            DoWork(0);

            using (new Tracer("Trace12"))
            {
                DoWork(0);
            }

            // shows how to use the shortcut methods for tracing
            _T("Trace13", () => DoWork(0));

            var value = _T("Trace14", () => DoWorkWithReturnValue(0));
        }

        using (new Tracer("Trace2"))
        {
            DoWork(0);
        }
    });

    var report = $"Traces:\n{Tracer.GetTraceEntries().AsMdReportString(Console.WindowWidth)}";
    Console.SetCursorPosition(0, cursorTop);
    Console.WriteLine(report);
} while (!Console.KeyAvailable);

return;

void DoWork(int foo)
{
    Thread.Sleep(100 + rnd.Next(50));
}

int DoWorkWithReturnValue(int foo)
{
    var millisecondsTimeout = 100 + rnd.Next(50);
    Thread.Sleep(millisecondsTimeout);
    return millisecondsTimeout;
}