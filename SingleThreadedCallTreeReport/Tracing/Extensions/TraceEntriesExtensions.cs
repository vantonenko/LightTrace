using ConsoleApp2.Tracing.Data;

namespace ConsoleApp2.Tracing.Extensions;

internal static class TraceEntriesExtensions
{
    internal static string AsMdReport(this TraceEntries traces) =>
        string.Join(
            "\n",
            new[]
                {
                    "| Path | Time, ms | Count |",
                    "| --- | --- | --- |"
                }
            .Concat(
                traces
                    .GetRecords()
                    .Select(line => $"| {line} |")));

    private static IEnumerable<string> GetRecords(this TraceEntries traces, string prefix = null)
    {
        foreach (KeyValuePair<string, TraceEntry> kvp in traces.OrderBy(t => t.Value.TimeSpan))
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