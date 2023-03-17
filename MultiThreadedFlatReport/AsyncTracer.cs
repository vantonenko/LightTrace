using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LightTrace
{
    public sealed class CallStackTrace : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private static AsyncContext RootCalls;
        private static AsyncLocal<AsyncContext> Context;

        static CallStackTrace()
        {
            Reset();
        }

        public CallStackTrace([CallerMemberName] string name = default)
        {
            var context = Context.Value ?? RootCalls;
            Context.Value = context.Calls.GetOrAdd(name, fName => new AsyncContext(fName, context));
            _stopwatch = Stopwatch.StartNew();
        }

        public static CallContext GetCalls()
        {
            return RootCalls.GetCallContext();
        }

        /// <summary>
        /// This method is used only for tests.
        /// </summary>
        public static void Reset()
        {
            RootCalls = new("Root", null);
            Context = new();
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _stopwatch.Stop();
            Context.Value.AddTime(_stopwatch.Elapsed);
            Context.Value = Context.Value.Parent;
        }

        #endregion

        private class AsyncContext
        {
            private long _totalTicks;
            private long _minTicks;
            private long _maxTicks;
            private int _callCount;
            private readonly string _name;

            public AsyncContext Parent { get; }
            public ConcurrentDictionary<string, AsyncContext> Calls { get; }

            internal AsyncContext(string name, AsyncContext parent)
            {
                _name = name;
                Parent = parent;
                _callCount= 0;
                _totalTicks = 0;
                _minTicks = 0;
                _maxTicks = 0;
                Calls = new ConcurrentDictionary<string, AsyncContext>();
            }

            internal void AddTime(TimeSpan time)
            {
                var ticks = time.Ticks;
                Interlocked.Add(ref _totalTicks, ticks);

                var callCount = Interlocked.Increment(ref _callCount);
                // Note: we do not guarantee that MinTicks and MaxTicks will be calculated correctly
                if (callCount == 1)
                {
                    Interlocked.Exchange(ref _minTicks, ticks);
                    Interlocked.Exchange(ref _maxTicks, ticks);
                }
                else
                {
                    if (Interlocked.Read(ref _minTicks) > ticks)
                    {
                        Interlocked.Exchange(ref _minTicks, ticks);
                    }

                    if (Interlocked.Read(ref _maxTicks) < ticks)
                    {
                        Interlocked.Exchange(ref _maxTicks, ticks);
                    }
                }
            }

            internal CallContext GetCallContext()
            {
                List<CallContext> calls = new List<CallContext>(Calls.Count);
                foreach (var call in Calls)
                {
                    var childCallContext = call.Value.GetCallContext();
                    calls.Add(childCallContext);
                }

                long totalTicks = Interlocked.Read(ref _totalTicks);
                long minTicks = Interlocked.Read(ref _minTicks);
                long maxTicks = Interlocked.Read(ref _maxTicks);
                var callContext = new CallContext(_name, totalTicks, minTicks, maxTicks, _callCount, calls);
                return callContext;
            }
        }
    }

    public class CallContext
    {
        public string Name { get; }
        public long TotalTicks { get; }
        public long MinTicks { get; }
        public long MaxTicks { get; }
        public int CallCount { get; }
        public IEnumerable<CallContext> Calls { get; }

        internal CallContext(string name, long totalTicks, long minTicks, long maxTicks, int callCount, IEnumerable<CallContext> calls)
        {
            Name = name;
            TotalTicks = totalTicks;
            MinTicks = minTicks;
            MaxTicks = maxTicks;
            CallCount= callCount;
            Calls = calls;
        }
    }

    public static class CallContextExtensions
    {
        public static IEnumerable<string> AsMdReportInTicks(this CallContext callContext, int minMilliseconds = 10) => callContext.AsMdReport(1, "ticks", minMilliseconds);
        public static IEnumerable<string> AsMdReportInMillisecond(this CallContext callContext, int minMilliseconds = 10) => callContext.AsMdReport(TimeSpan.TicksPerMillisecond, "ms", minMilliseconds);

        private static IEnumerable<string> AsMdReport(this CallContext callContext, long coefficient, string coefficientName,  int minMilliseconds = 10)
        {
            return new[]
            {
                $"| Query | Total ({coefficientName}) | Count | Avg ({coefficientName}) | Min ({coefficientName}) | Max ({coefficientName}) | Spread ({coefficientName}) |",
                "| --- | --- | --- | --- | --- | --- | --- |"
            }.Concat(callContext.AsMdLineReport(coefficient, minMilliseconds, 1));
        }

        private static IEnumerable<string> AsMdLineReport(this CallContext callContext, long coefficient, int minMilliseconds, int callLevel)
        {
            var totalMilliseconds = callContext.TotalTicks / TimeSpan.TicksPerMillisecond;
            if (totalMilliseconds > 0  && totalMilliseconds < minMilliseconds)
            {
                return Enumerable.Empty<string>();
            }

            var avrTicks = callContext.CallCount > 0 ? callContext.TotalTicks / callContext.CallCount : callContext.TotalTicks;
            var total = callContext.TotalTicks / coefficient;
            var avr = avrTicks / coefficient;
            var min = callContext.MinTicks / coefficient;
            var max = callContext.MaxTicks / coefficient;
            return new[]
            {
                $"| {new string(' ', callLevel)}{callContext.Name} | {total} | {callContext.CallCount} | {avr} | {min} | {max} | {max - min} |"
            }.Concat(callContext.Calls.Where(o => o.TotalTicks > minMilliseconds).SelectMany(o => o.AsMdLineReport(coefficient, minMilliseconds, callLevel + 1)));
        }
    }
}
