using System.Collections.Generic;
using System.Linq;
using LightTrace.Data;

namespace LightTrace.Extensions;

public static class TraceTraceEntrySnapshotExtensions
{
    public static IEnumerable<string> AsMdReport(this TraceEntrySnapshots traces) =>
        new[]
            {
                "| Path | Time | Count |",
                "| --- | --- | --- |"
            }
            .Concat(
                traces
                    .GetRecords()
                    .Select(line => $"| {line} |"));

    public static string AsMdReportString(this TraceEntrySnapshots traces) => string.Join("\n", traces.AsMdReport());

    private static IEnumerable<string> GetRecords(this TraceEntrySnapshots traces, string prefix = null)
    {
        foreach (KeyValuePair<string, TraceEntrySnapshot> kvp in traces.OrderByDescending(t => t.Value.TimeSpan))
        {
            TraceEntrySnapshot traceEntry = kvp.Value;
            string currentPrefix = $" --- {prefix}";

            yield return $"{currentPrefix}{kvp.Key} | {traceEntry.TimeSpan.AsTime()} | {traceEntry.Count.AsCount()}";

            foreach (string record in traceEntry.TraceEntries.GetRecords(currentPrefix))
            {
                yield return record;
            }
        }
    }
}
