using System.Collections.Concurrent;

namespace ConsoleApp2.Tracing.Data;

internal class TraceEntries : ConcurrentDictionary<string, TraceEntry>
{
}