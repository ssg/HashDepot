using BenchmarkDotNet.Running;

namespace benchmark;

class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<BenchmarkSuite>();
    }
}