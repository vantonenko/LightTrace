using BenchmarkDotNet.Attributes;
using LightTrace;

namespace TraceBenchmark
{
    [MemoryDiagnoser]
    public class BenchmarkV1
    {
        private const string FunctionLevel0 = "FunctionLevel0";
        private const int MaxIterations = 1000;
        private const int MaxDeep = 1000;
        private readonly string[,] _functionNames = new string[MaxIterations, MaxDeep];

        [Params(10, 100)]
        public int Deep { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            for (int i = 0; i < MaxIterations; i++)
            {
                for (int j = 0; j < MaxDeep; j++)
                {
                    _functionNames[i,j] = $"{FunctionLevel0}_{i}_{j}";
                }
            }
        }

        [IterationSetup]
        public void IterationSetup()
            => CallStackTrace.Reset();


        [Benchmark]
        public async Task AllUniqueFunctionName()
        {
            for (int i = 0; i < MaxIterations; i++)
            {
                await CallFunctionAsync(i, Deep);
            }
        }

        [Benchmark]
        public async Task UniqueFunctionName()
        {
            for (int i = 0; i < MaxIterations; i++)
            {
                await CallFunctionAsync(0, Deep);
            }
        }

        private async Task CallFunctionAsync(int iteration, int callDeep, int currentDeep = 0)
        {
            using (new CallStackTrace(_functionNames[iteration, currentDeep]))
            {
                currentDeep++;
                if (callDeep > currentDeep)
                {
                    await CallFunctionAsync(iteration, callDeep, currentDeep);
                }
            }
        }
    }
}
