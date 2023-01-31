using System.Diagnostics;
using ConsoleApp2.Tracing.Data;
using ConsoleApp2.Tracing.Extensions;

namespace ConsoleApp2.Tracing;

internal sealed class Tracer : IDisposable
{
    private static Stack<TraceEntry> ParentTraceEntriesStack { get; } = new();
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

    public static TraceEntries GetRootTraceEntries() => RootTraceEntries;
}