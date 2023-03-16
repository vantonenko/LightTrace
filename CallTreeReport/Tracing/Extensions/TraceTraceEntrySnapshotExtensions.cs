using ConsoleApp2.Tracing.Data;

namespace ConsoleApp2.Tracing.Extensions;

internal static class TraceTraceEntrySnapshotExtensions
{
    internal static IEnumerable<string> AsMdReport(this TraceEntrySnapshots traces) =>
        new[]
            {
                "| Path | Time, ms | Count |",
                "| --- | --- | --- |"
            }
            .Concat(
                traces
                    .GetRecords()
                    .Select(line => $"| {line} |"));

    internal static string AsMdReportString(this TraceEntrySnapshots traces) => string.Join("\n", traces.AsMdReport());

    private static IEnumerable<string> GetRecords(this TraceEntrySnapshots traces, string prefix = null)
    {
        foreach (KeyValuePair<string, TraceEntrySnapshot> kvp in traces.OrderBy(t => t.Value.TimeSpan))
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