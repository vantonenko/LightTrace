using ConsoleApp2.Tracing.Data;

namespace ConsoleApp2.Tracing.Extensions;

internal static class TraceEntriesExtensions
{
    internal static IEnumerable<string> AsMdReport(this TraceEntries traces) =>
        new[]
            {
                "| Path | Time, ms | Count |",
                "| --- | --- | --- |"
            }
            .Concat(
                traces
                    .GetRecords()
                    .Select(line => $"| {line} |"));

    internal static string AsMdReportString(this TraceEntries traces) => string.Join("\n", traces.AsMdReport());

    private static IEnumerable<string> GetRecords(this TraceEntries traces, string prefix = null)
    {
        KeyValuePair<string, TraceEntry>[] threadSafePairs = traces.ToArray();

        foreach (KeyValuePair<string, TraceEntry> kvp in threadSafePairs.OrderBy(t => t.Value.TimeSpan))
        {
            TraceEntry traceEntry = kvp.Value;
            string currentPrefix = $" --- {prefix}";

            yield return $"{currentPrefix}{kvp.Key} | {traceEntry.TimeSpan.AsTime()} | {traceEntry.Count.AsCount()}";

            foreach (string record in traceEntry.TraceEntries.GetRecords(currentPrefix))
            {
                yield return record;
            }
        }
    }
}