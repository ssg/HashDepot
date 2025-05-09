using BenchmarkDotNet.Running;

namespace benchmark;

class Program
{
    public static void Main()
    {
        _ = BenchmarkRunner.Run<BenchmarkSuite>();
    }
}