namespace ConsoleApp2.Tracing.Data;

internal class TraceEntrySnapshots : Dictionary<string, TraceEntrySnapshot>
{
    public TraceEntrySnapshots(IEnumerable<KeyValuePair<string, TraceEntrySnapshot>> keyValuePairs) : base(keyValuePairs)
    {
    }
}