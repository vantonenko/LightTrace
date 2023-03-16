using System.Collections.Generic;

namespace LightTrace.Data;

public class TraceEntrySnapshots : Dictionary<string, TraceEntrySnapshot>
{
    public TraceEntrySnapshots(IDictionary<string, TraceEntrySnapshot> dictionary) : base(dictionary)
    {
    }
}