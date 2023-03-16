using System.Diagnostics;
using ConsoleApp2.Tracing.Data;
using ConsoleApp2.Tracing.Extensions;

namespace ConsoleApp2.Tracing;

internal sealed class Tracer : IDisposable
{
    private static readonly AsyncLocal<Stack<TraceEntry>> AsyncLocal = new();

    private static Stack<TraceEntry> TraceEntryStack => AsyncLocal.Value ??= new Stack<TraceEntry>();
    
    private static readonly TraceEntries RootTraceEntries = new();

    private readonly TraceEntry _currentTraceEntry;
    private readonly Stopwatch _stopwatch;

    public Tracer(string name)
    {
        _currentTraceEntry = GetCurrentTraceEntry(name);

        _stopwatch = Stopwatch.StartNew();

        TraceEntryStack.Push(_currentTraceEntry);
    }

    private static TraceEntry GetCurrentTraceEntry(string name)
    {
        TraceEntries parentTraceEntries = TraceEntryStack.PeekOrDefault()?.TraceEntries ?? RootTraceEntries;

        return parentTraceEntries.GetOrAdd(name, _ => new TraceEntry());
    }

    public void Dispose()
    {
        Interlocked.Increment(ref _currentTraceEntry.Count);
        Interlocked.Add(ref _currentTraceEntry.Ticks, _stopwatch.ElapsedTicks);

        TraceEntryStack.Pop();
    }

    public static TraceEntries GetRootTraceEntries() => RootTraceEntries;
}