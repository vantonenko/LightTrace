namespace ConsoleApp2.Tracing.Data;

internal class TraceEntry
{
    public int Count;
    
    public long Ticks = 0;

    public TimeSpan TimeSpan => TimeSpan.FromTicks(Ticks);
    
    public TraceEntries TraceEntries { get; set; } = new();
}

