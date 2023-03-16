namespace ConsoleApp2.Tracing.Data;

internal class TraceEntrySnapshot
{
    public int Count;
    
    public long Ticks;

    public TimeSpan TimeSpan => TimeSpan.FromTicks(Ticks);

    public TraceEntrySnapshots TraceEntries { get; set; }
}