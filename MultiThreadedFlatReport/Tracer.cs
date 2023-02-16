using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using LightTrace.Extensions;
using Newtonsoft.Json;

namespace LightTrace
{
    public sealed class Tracer : IDisposable
    {
        private readonly string _name;
        private readonly IEnumerable<(string name, string value)> _parameters;
        private readonly Stopwatch _stopwatch;

        private static readonly ConcurrentDictionary<
            string,
            (
                int count,
                TimeSpan totalTime,
                TimeSpan min,
                TimeSpan max,
                IEnumerable<(string name, string value)> lastParameters
            )> AggregatedTraces = new();

        private static readonly TimeSpan ReportInterval = TimeSpan.FromSeconds(15);
        private static readonly string ReportFolder = Environment.GetEnvironmentVariable("Temp");
        private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;

        static Tracer()
        {
            new Thread(DoReport)
            {
                IsBackground = true
            }.Start();
        }

        public Tracer(string name, IEnumerable<(string name, string value)> parameters = null)
        {
            _name = name;
            _parameters = parameters ?? Enumerable.Empty<(string, string)>();
            _stopwatch = Stopwatch.StartNew();
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _stopwatch.Stop();
            AggregatedTraces.AddOrUpdate(
                _name,
                (1, _stopwatch.Elapsed, _stopwatch.Elapsed, _stopwatch.Elapsed, _parameters),
                (_, existingTrace) =>
                (
                    existingTrace.count + 1,
                    existingTrace.totalTime + _stopwatch.Elapsed,
                    existingTrace.min.Min(_stopwatch.Elapsed),
                    existingTrace.max.Max(_stopwatch.Elapsed),
                    _stopwatch.Elapsed > existingTrace.max
                        ? _parameters
                        : existingTrace.lastParameters
                ));
        }

        #endregion

        private static void DoReport()
        {
            try
            {
                do
                {
                    UpdateReportFile();
                    UpdateParametersFile();
                    Thread.Sleep(ReportInterval);
                }
                while (true);
                // ReSharper disable once FunctionNeverReturns
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }

        private static void UpdateParametersFile() =>
            File.WriteAllText(
                Path.Combine(ReportFolder, $"{ProcessName}_Traces_MaxCallTimeParameters.json"),
                JsonConvert.SerializeObject(
                    AggregatedTraces
                        .ToArray()// enumerate in a thread safe way
                        .OrderByDescending(o => o.Value.totalTime)
                        .ToDictionary(
                            o => o.Key,
                            o => o.Value.lastParameters.ToDictionary(p => p.name, p => p.value)),
                    Formatting.Indented));

        private static void UpdateReportFile() =>
            File.WriteAllLines(
                Path.Combine(ReportFolder, $"{ProcessName}_Traces.md"),
                AggregatedTraces
                    .ToArray()// enumerate in a thread safe way
                    .OrderByDescending(o => o.Value.totalTime)
                    .ToDictionary(
                        o => o.Key,
                        o => o.Value)
                    .AsMdReport());
    }
}
