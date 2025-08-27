using System;
using System.Collections.Generic;
using System.Linq;

namespace LightTrace.Extensions
{
    internal static class TracesExtensions
    {
        public static IEnumerable<string> AsMdReport(
            this Dictionary<string, (int count, TimeSpan totalTime, TimeSpan min, TimeSpan max, IEnumerable<(string name, string value)> lastParameters)> traces,
            int minMilliseconds = 10) =>
            new[]
            {
                "| Query | Total | Count | Avg | Min | Max |",
                "| --- | --- | --- | --- | --- | --- |"
            }.Concat(
                traces
                    .Where(o => o.Value.totalTime >= TimeSpan.FromMilliseconds(minMilliseconds))
                    .Select(o => new
                    {
                        Query = o.Key.TakeFirst(50),
                        o.Value.totalTime,
                        o.Value.count,
                        avg = o.Value.totalTime.TotalMilliseconds / o.Value.count,
                        o.Value.min,
                        o.Value.max
                    })
                    .Select(o
                        => $"| {o.Query} | {o.totalTime.AsTime()} | {o.count.AsCount()} | {o.avg.AsTime()} | {o.min.AsTime()} | {o.max.AsTime()} |"));
    }
}
