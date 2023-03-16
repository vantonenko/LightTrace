﻿using System.Diagnostics;
using ConsoleApp2.Tracing.Data;
using ConsoleApp2.Tracing.Extensions;

namespace ConsoleApp2.Tracing;

internal sealed class Tracer : IDisposable
{
    private static readonly AsyncLocal<TracingThreadContext> AsyncLocal = new();

    private static TracingThreadContext TracingThreadContext => 
        AsyncLocal.Value ?? (AsyncLocal.Value = new TracingThreadContext());

    private static Stack<TraceEntry> TraceEntriesStack => TracingThreadContext.ParentTraceEntriesStack;
    private static readonly TraceEntries RootTraceEntries = new();

    private readonly TraceEntry _currentTraceEntry;
    private readonly Stopwatch _stopwatch;

    public Tracer(string name)
    {
        TraceEntries parentTraceEntries =
            TraceEntriesStack
                .PeekOrDefault()
                ?.TraceEntries
            ?? RootTraceEntries;

        _currentTraceEntry = parentTraceEntries.GetOrAdd(name, _ => new TraceEntry());

        _stopwatch = Stopwatch.StartNew();

        TraceEntriesStack.Push(_currentTraceEntry);
    }

    public void Dispose()
    {
        Interlocked.Increment(ref _currentTraceEntry.Count);
        Interlocked.Add(ref _currentTraceEntry.Ticks, _stopwatch.ElapsedTicks);

        TraceEntriesStack.Pop();
    }

    public static TraceEntries GetRootTraceEntries() => RootTraceEntries;
}