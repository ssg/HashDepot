using HashDepot;
using BenchmarkDotNet.Attributes;

namespace benchmark;

public class BenchmarkSuite
{
    public const int BufSize = 1001 * 1003;

    public int Bits;

    private static readonly byte[] buf = new byte[BufSize];
    private static readonly byte[] sipHashKey = new byte[16];

#pragma warning disable CA1822 // Mark members as static - BenchmarkDotNet requires these as instance members

    [Benchmark]
    public void Checksum_32() => Checksum.Hash32(buf);

    [Benchmark]
    public void XXHash_32() => XXHash.Hash32(buf, 0);

    [Benchmark]
    public void XXHash_64() => XXHash.Hash64(buf, 0);

    [Benchmark]
    public void MurmurHash3_x86() => MurmurHash3.Hash32(buf, 0);

    [Benchmark]
    public void SipHash24_32() => SipHash24.Hash64(buf, sipHashKey);

    [Benchmark]
    public void Fnv1a_32() => Fnv1a.Hash32(buf);

    [Benchmark]
    public void Fnv1a_64() => Fnv1a.Hash64(buf);

#pragma warning restore CA1822 // Mark members as static
}
