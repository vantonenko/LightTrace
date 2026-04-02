using System.Collections.Generic;
using System.Linq;
using LightTrace.Data;

namespace LightTrace.Extensions;

public static class TraceTraceEntrySnapshotExtensions
{
    extension(TraceEntrySnapshots traces)
    {
        public IEnumerable<string> AsMdReport(int padLength = 0) =>
            new[]
                {
                    "| Path | Time | Count |",
                    "| --- | --- | --- |"
                }
                .Concat(
                    traces
                        .GetRecords()
                        .Select(line => $"| {line} |".PadRight(padLength)));

        public string AsMdReportString(int padLength = 0) => string.Join("\n", traces.AsMdReport(padLength));

        private IEnumerable<string> GetRecords(string prefix = null)
        {
            foreach (var kvp in traces.OrderByDescending(t => t.Value.TimeSpan))
            {
                var traceEntry = kvp.Value;
                var currentPrefix = $" --- {prefix}";

                yield return $"{currentPrefix}{kvp.Key} | {traceEntry.TimeSpan.AsTime()} | {traceEntry.Count.AsCount()}";

                foreach (var record in traceEntry.TraceEntries.GetRecords(currentPrefix))
                {
                    yield return record;
                }
            }
        }
    }
}
