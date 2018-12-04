using System;
using HashDepot;

namespace benchmark
{
    class Program
    {
        private static readonly byte[] sipHashKey = new byte[16];

        private static readonly Benchmark[] benchmarks = new[]
        {
            new Benchmark("Checksum", 32, (buf) => Checksum.Hash32(buf)),
            new Benchmark("Fnv1a", 32, (buf) => Fnv1a.Hash32(buf)),
            new Benchmark("Fnv1a", 64, (buf) => Fnv1a.Hash64(buf)),
            new Benchmark("MurmurHash3", 32, (buf) => MurmurHash3.Hash32(buf, 0)),
            new Benchmark("SipHash", 64, (buf) => SipHash.Hash64(buf, sipHashKey)),
        };

        public static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("WARNING: DEBUG MODE");
            Console.WriteLine();
#endif 
            Console.WriteLine("{0} iterations over {1} bytes of buffer", 
                Benchmark.Iterations, Benchmark.BufSize);
            Console.WriteLine(@"
Name                  | Ops/sec
----------------------|---------------------------");
            var baseline = benchmarks[0];
            foreach (var benchmark in benchmarks)
            {
                Console.Write("{0,-22}| ", benchmark.Name + " (" + benchmark.Bits + "-bit)");
                Console.Write("testing...");
                benchmark.Test();
                Console.Write(new String('\x08', 10));
                Console.WriteLine("{0,10:#.00}", Benchmark.Iterations * 1000 / benchmark.TimeTaken.TotalMilliseconds);
            }
            Console.WriteLine();
        }
    }
}