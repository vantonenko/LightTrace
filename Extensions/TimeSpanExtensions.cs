using System;

namespace LightTrace.Extensions
{
    internal static class TimeSpanExtensions
    {
        internal static TimeSpan Min(this TimeSpan a, TimeSpan b) => a < b ? a : b;

        internal static TimeSpan Max(this TimeSpan a, TimeSpan b) => a > b ? a : b;
    }
}
