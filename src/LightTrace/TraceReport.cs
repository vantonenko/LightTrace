using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LightTrace.Extensions;

namespace LightTrace;

public class TraceReport
{
    private static readonly TimeSpan ReportInterval = GetReportInterval();
    private static readonly string ReportFolder = GetReportFolder();
    private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;

    public static string ReportFile { get; } = Path.Combine(ReportFolder, $"{ProcessName}_Traces.md");

    public static void Start()
    {
        new Thread(DoReport)
        {
            IsBackground = true
        }.Start();
    }

    private static TimeSpan GetReportInterval()
    {
        var intervalEnvVar = Environment.GetEnvironmentVariable("LIGHT_TRACE_REPORT_INTERVAL");
        if (!string.IsNullOrEmpty(intervalEnvVar) && double.TryParse(intervalEnvVar, out var seconds))
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return TimeSpan.FromSeconds(15);
    }

    private static string GetReportFolder()
    {
        var folderEnvVar = Environment.GetEnvironmentVariable("LIGHT_TRACE_REPORT_FOLDER");
        if (string.IsNullOrEmpty(folderEnvVar))
        {
            folderEnvVar = Environment.GetEnvironmentVariable("Temp");
        }

        return folderEnvVar;
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