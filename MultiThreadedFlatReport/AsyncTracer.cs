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
    public sealed class AsyncTracer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        //public static readonly AsyncContext RootCallContext = new AsyncContext("Root", null);
        public static readonly ConcurrentDictionary<string, AsyncContext> Calls = new();
        private static readonly AsyncLocal<AsyncContext> Context = new();

        public AsyncTracer(string name)
        {
            Context.Value = Context.Value == null
                ? Calls.GetOrAdd(name, fName => new AsyncContext(fName, null))
                : Context.Value.Calls.GetOrAdd(name, fName => new AsyncContext(fName, Context.Value));
            //if (Context.Value == null)
            //{
            //    Context.Value = RootCallContext.Calls.GetOrAdd(name, fName => new AsyncContext(fName, null));
            //}
            //else
            //{
            //    var context = Context.Value;
            //    Context.Value = context.Calls.GetOrAdd(name, fName => new AsyncContext(fName, context));
            //}
            _stopwatch = Stopwatch.StartNew();
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

        public class AsyncContext
        {
            private long _totalTicks;
            
            public string Name { get; }
            public long TotalTicks => _totalTicks;
            public AsyncContext Parent { get; }
            public ConcurrentDictionary<string, AsyncContext> Calls { get; }

            internal AsyncContext(string name, AsyncContext parent)
            {
                Name = name;
                Parent = parent;
                Calls = new ConcurrentDictionary<string, AsyncContext>();
            }

            internal void AddTime(TimeSpan time)
            {
                Interlocked.Add(ref _totalTicks, time.Ticks);
            }
        }
    }
}
