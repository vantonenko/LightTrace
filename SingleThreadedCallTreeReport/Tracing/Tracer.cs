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
    private static readonly TraceEntries RootTraceEntries = new();

    private readonly TraceEntry _currentTraceEntry;
    private readonly Stopwatch _stopwatch;

    public Tracer(string name)
    {
        TraceEntries parentTraceEntries =
            ParentTraceEntriesStack
                .PeekOrDefault()
                ?.TraceEntries
            ?? RootTraceEntries;

        // todo make this thread-safe
        if (parentTraceEntries.TryGetValue(name, out TraceEntry val))
        {
            _currentTraceEntry = val;
        }
        else
        {
            _currentTraceEntry = parentTraceEntries[name] = new TraceEntry();
        }

        _stopwatch = Stopwatch.StartNew();

        ParentTraceEntriesStack.Push(_currentTraceEntry);
    }

    public void Dispose()
    {
        Interlocked.Increment(ref _currentTraceEntry.Count);
        Interlocked.Add(ref _currentTraceEntry.Ticks, _stopwatch.ElapsedTicks);

        ParentTraceEntriesStack.Pop();
    }

    public static TraceEntries GetRootTraceEntries() => RootTraceEntries;
}