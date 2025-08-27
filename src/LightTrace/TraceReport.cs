using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LightTrace.Extensions;

namespace LightTrace;

public class TraceReport
{
    private static readonly TimeSpan ReportInterval = TimeSpan.FromSeconds(15);
    private static readonly string ReportFolder = Environment.GetEnvironmentVariable("Temp");
    private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;

    public static string ReportFile { get; } = Path.Combine(ReportFolder, $"{ProcessName}_Traces.md");

    public static void Start()
    {
        new Thread(DoReport)
        {
            IsBackground = true
        }.Start();
    }

    private static void DoReport()
    {
        do
        {
            UpdateReportFile();
            Thread.Sleep(ReportInterval);
        } while (true);
        // ReSharper disable once FunctionNeverReturns
    }

    private static void UpdateReportFile() =>
        File.WriteAllLines(ReportFile, Tracer.GetTraceEntries().AsMdReport());
}