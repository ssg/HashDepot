# HashDepot
[![NuGet Version](https://img.shields.io/nuget/v/HashDepot.svg)](https://www.nuget.org/packages/HashDepot/)
![Build Status](https://github.com/ssg/HashDepot/actions/workflows/build.yml/badge.svg)

I have been implementing various hash functions that are absent in .NET framework. 
I decided to converge them into a library. My primary goals are to provide well-tested and 
performant implementations. The library currently supports [SipHash](https://131002.net/siphash/),
[MurmurHash3](https://en.wikipedia.org/wiki/MurmurHash), [xxHash](http://cyan4973.github.io/xxHash/)
and [FNV-1a](https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1a_hash). 

To install it on NuGet:

    Install-Package HashDepot

# Supported Hash Algorithms
I try to add anything that's not in C# runtime and quite popular. For instance,
there are multiple xxHash implementations for C# but they differentiate in terms of API
complexity and performance. Although I didn't try out SIMD optimizations, the existing code
is quite fast.

## xxHash
This one claims to be one of the fastest hash functions and it's actually amazing. Even without any SIMD
optimizations, it outperforms everything, even a plain checksum by a factor of two. The implementation
assumes little endian machines. Example usage:

```csharp
var buffer = Encoding.UTF8.GetBytes("some string");
uint result = XXHash.Hash32(buffer, seed: 123);
ulong result = XXHash.Hash64(buffer); // default seed is zero
```

## SipHash
SipHash is resistant to hash-flood attacks against hashtables and uses
a key parameter to ensure HMAC-like authenticity yet faster. Unfortuantely a native 
.NET implementation does not exist. It is my take on it, and it is really fast for a 
managed environment. It's standard SipHash-2-4 implementation with 64-bit. To use it:

```csharp
var buffer = Encoding.UTF8.GetBytes("some string");
var key = new byte[16] { .. your random key here .. };
ulong result = SipHash24.Hash64(buffer, key);
```

If you have a larger buffer than 2GB it's better to use streaming functions instead.

## MurmurHash3
MurmurHash3 provides a good balance between performance and homogenity but is 
essentially prone to hash-flood attacks (trivial to force collisions). HashDepot
implements its x86 flavor (not x64). An example use is:

```csharp
var buffer = Encoding.UTF8.GetBytes("some string");
uint seed = // .. preferred seed value ...
uint result = MurmurHash3.Hash32(buffer, seed);
```

## FNV-1a
A straightforward implementation of FNV-1a hash algorithm for .NET. Usage is 
very simple. For instance to calculate 32-bit FNV-1a hash of ASCII string "some string":

```csharp
var buffer = Encoding.UTF8.GetBytes("some string");
uint result = Fnv1a.Hash32(buffer); // 32-bit hash
ulong result = Fnv1a.Hash64(buffer); // 64-bit hash
```
  
## Streaming and Async functions
All hashes also provide stream-based (albeit slow) functions with their async variants too. In order to
get the hash of a stream just call the function with a stream instead of a memory buffer:

```csharp
ulong result = XXHash.Hash64(stream);
```

If you'd like to run it asynchronously, use the async variant:

```csharp
uint result = await MurmurHash3.Hash32Async(stream);
```

# Benchmarks
Benchmarks are performed on a 1MB buffer.

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4202/24H2/2024Update/HudsonValley)
AMD Ryzen 9 5950X 4.00GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.300
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2


| Method          | Mean        | Error    | StdDev   | Allocated |
|---------------- |------------:|---------:|---------:|----------:|
| Checksum_32     |    95.80 us | 1.507 us | 1.336 us |         - |
| XXHash_32       |   201.34 us | 1.353 us | 1.266 us |         - |
| XXHash_64       |   185.41 us | 0.759 us | 0.634 us |         - |
| MurmurHash3_x86 |   288.00 us | 0.820 us | 0.684 us |         - |
| SipHash24_32    |   284.99 us | 0.881 us | 0.736 us |         - |
| Fnv1a_32        | 1,009.49 us | 0.144 us | 0.121 us |       1 B |
| Fnv1a_64        | 1,009.56 us | 0.136 us | 0.113 us |       1 B |

# Contributing
You're more than welcome to contribute fixes or new hash algorithms. Please keep these in mind:

- Make sure the code builds without warnings.
- Include unit tests for the fixed bug, or the new feature.
- If you're proposing a new hash algorithm, please make sure that it's not in C# runtime, there isn't an
  existing library that is already tremendously popular, and HashDepot's simplistic approach would provide
  a benefit over the alternatives.

# License
MIT License. See LICENSE file for details
