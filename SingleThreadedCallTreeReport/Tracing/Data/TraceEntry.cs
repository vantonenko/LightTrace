namespace ConsoleApp2.Tracing.Data;

internal class TraceEntry
{
    public int Count { get; set; }
    public TimeSpan TimeSpan { get; set; } = TimeSpan.Zero;
    public TraceEntries TraceEntries { get; set; } = new();
}