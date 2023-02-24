using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LightTrace
{
    public sealed class CallStackTrace : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private static readonly AsyncContext RootCalls = new ("Root", null);
        private static readonly AsyncLocal<AsyncContext> Context = new();

        public CallStackTrace(string name)
        {
            var context = Context.Value ?? RootCalls;
            Context.Value = context.Calls.GetOrAdd(name, fName => new AsyncContext(fName, context));
            _stopwatch = Stopwatch.StartNew();
        }

        public static CallContext GetCalls()
        {
            return RootCalls.GetCallContext();
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
            private readonly string _name;

            public AsyncContext Parent { get; }
            public ConcurrentDictionary<string, AsyncContext> Calls { get; }

            internal AsyncContext(string name, AsyncContext parent)
            {
                _name = name;
                Parent = parent;
                Calls = new ConcurrentDictionary<string, AsyncContext>();
            }

            internal void AddTime(TimeSpan time)
            {
                Interlocked.Add(ref _totalTicks, time.Ticks);
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
                var callContext = new CallContext(_name, totalTicks, calls);
                return callContext;
            }
        }
    }

    public class CallContext
    {
        public string Name { get; }
        public long TotalTicks { get; }
        public IEnumerable<CallContext> Calls { get; }

        internal CallContext(string name, long totalTicks, IEnumerable<CallContext> calls)
        {
            Name = name;
            TotalTicks = totalTicks;
            Calls = calls;
        }
    }
}
