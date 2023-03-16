using System.Collections.Concurrent;
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

    static Tracer() => TraceReport.Start();

    public Tracer(string name)
    {
        _currentTraceEntry = GetOrCreateCurrentTraceEntry(name);

        _stopwatch = Stopwatch.StartNew();

        TraceEntryStack.Push(_currentTraceEntry);
    }

    public void Dispose()
    {
        Interlocked.Increment(ref _currentTraceEntry.Count);
        Interlocked.Add(ref _currentTraceEntry.Ticks, _stopwatch.ElapsedTicks);

        TraceEntryStack.Pop();
    }

    public static TraceEntrySnapshots GetTraceEntries() => RootTraceEntries.GetTraceSnapshot();

    private static TraceEntry GetOrCreateCurrentTraceEntry(string name)
    {
        TraceEntries parentTraceEntries = TraceEntryStack.PeekOrDefault()?.TraceEntries ?? RootTraceEntries;

        return parentTraceEntries.GetOrAdd(name, _ => new TraceEntry());
    }

    private class TraceEntry
    {
        public int Count;

        public long Ticks;

        public TraceEntries TraceEntries { get; } = new();
    }

    private class TraceEntries : ConcurrentDictionary<string, TraceEntry>
    {
        public TraceEntrySnapshots GetTraceSnapshot() =>
            new(
                ToArray() // thread-safe enumeration, http://blog.i3arnon.com/2018/01/16/concurrent-dictionary-tolist/
                    .Select(o =>
                        new KeyValuePair<string, TraceEntrySnapshot>(
                            o.Key,
                            new TraceEntrySnapshot
                            {
                                Count = o.Value.Count,
                                Ticks = Interlocked.Read(ref o.Value.Ticks), // 64 bit read isn't atomic
                                TraceEntries = o.Value.TraceEntries.GetTraceSnapshot()
                            })));

    }
}