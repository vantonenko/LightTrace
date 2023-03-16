namespace ConsoleApp2.Tracing.Data;

internal class TracingThreadContext
{
    public Stack<TraceEntry> ParentTraceEntriesStack { get; } = new();
}