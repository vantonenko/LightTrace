using System.Diagnostics;
using ConsoleApp2.Tracing.Data;
using ConsoleApp2.Tracing.Extensions;

namespace ConsoleApp2.Tracing;

internal sealed class Tracer : IDisposable
{
    private static readonly ThreadLocal<TracingThreadContext> ThreadLocal = 
        new(() => new TracingThreadContext(), true);

    private static TracingThreadContext TracingThreadContext => ThreadLocal.Value;

    private static Stack<TraceEntry> ParentTraceEntriesStack => TracingThreadContext.ParentTraceEntriesStack;
    private static TraceEntries RootTraceEntries => TracingThreadContext.RootTraceEntries;

    private readonly TraceEntry _currentTraceEntry;
    private readonly Stopwatch _stopwatch;

    public Tracer(string name)
    {
        TraceEntries parentTraceEntries =
            ParentTraceEntriesStack
                .PeekOrDefault()
                ?.TraceEntries
            ?? RootTraceEntries;

        _currentTraceEntry =
            parentTraceEntries
                .TryGetValue(name, out TraceEntry val)
                    ? val :
                    parentTraceEntries[name] = new TraceEntry();

        _stopwatch = Stopwatch.StartNew();

        ParentTraceEntriesStack.Push(_currentTraceEntry);
    }

    public void Dispose()
    {
        _currentTraceEntry.Count++;
        _currentTraceEntry.TimeSpan += _stopwatch.Elapsed;
        ParentTraceEntriesStack.Pop();
    }

    /// <summary>
    /// Get all the aggregated traces per thread.
    /// </summary>
    /// <remarks>The method isn't thread safe. Accessing that while the intensive trace collection may result in exceptions.</remarks>
    public static IEnumerable<TraceEntries> GetRootTraceEntries() => 
        ThreadLocal.Values.Select(o => o.RootTraceEntries);
}