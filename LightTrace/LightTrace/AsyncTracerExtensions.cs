using System.Collections.Generic;
using System.Linq;

namespace LightTrace
{
    public static class AsyncTracerExtensions
    {
        public static IEnumerable<string> AsMdReport(this CallContext traces)
        {
            return GetMdLines(traces, 0);
        }

        private static IEnumerable<string> GetMdLines(this CallContext traces, int callLevel)
        {
            if (traces == null)
            {
                return Enumerable.Empty<string>();
            }
            var lines = new List<string>();
            var line = $"| +{new string('-', callLevel)} {traces.Name} | {traces.CallCount} | {traces.MaxTicks} | {traces.MinTicks} | {traces.TotalTicks} |";
            lines.Add(line);

            foreach (var callContext in traces.Calls)
            {
                lines.AddRange(GetMdLines(callContext, callLevel + 1));
            }

            return lines;

        }
    }
}
