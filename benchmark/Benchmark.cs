using System;
using System.Diagnostics;

namespace benchmark;

class Benchmark
{
    public const int Iterations = 10_000;
    public const int BufSize = 1001 * 1003;

    public string Name;
    public int Bits;

    public Action<byte[]> HashFunc;

    public TimeSpan TimeTaken;

    public Benchmark(string name, int resultBits, Action<byte[]> func)
    {
        this.Name = name;
        this.HashFunc = func;
        this.Bits = resultBits;
    }

    public void Test()
    {
        var buf = new byte[BufSize];
        var w = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            HashFunc(buf);
        }
        w.Stop();
        this.TimeTaken = w.Elapsed;
    }
}