# HashDepot
[![NuGet Version](https://img.shields.io/nuget/v/HashDepot.svg)](https://www.nuget.org/packages/HashDepot/)
[![Build Status](https://travis-ci.org/ssg/HashDepot.svg?branch=master)](https://travis-ci.org/ssg/HashDepot)

I have been implementing various hash functions that are absent in .NET framework. 
I decided to converge them into a library. My primary goals are to provide well-tested and 
performant implementations. The library currently supports [SipHash](https://131002.net/siphash/),
[MurmurHash3](https://en.wikipedia.org/wiki/MurmurHash),
[FNV-1](https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1_hash) and [FNV-1a](https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1a_hash). 

To install it on NuGet:

    Install-Package HashDepot

## SipHash
SipHash is resistant to hash-flood attacks against hashtables and uses
a key parameter to ensure HMAC-like authenticity yet faster. Unfortuantely a native 
.NET implementation does not exist. It is my take on it, and it is really fast for a 
managed environment. It's standard SipHash-2-4 implementation with 64-bit. To use it:

```csharp
using HashDepot;
var buffer = Encoding.ASCII.GetBytes("some string");
var key = new byte[16] { .. your random key here .. };
ulong result = SipHash.Hash64(buffer, key);
```

If you have a larger buffer than 2GB it's better to use streaming functions instead.

## MurmurHash3
MurmurHash3 provides a good balance between performance and homogenity but is 
essentially prone to hash-flood attacks (trivial to force collisions). An example use is:

```csharp
using HashDepot;
var buffer = Encoding.ASCII.GetBytes("some string");
uint seed = .. preferred seed value ...
uint result = MurmurHash3.Hash32(buffer, seed);
```

## FNV
A straightforward implementation of FNV-1 and FNV-1a hash algorithm for .NET. Usage is 
very simple. For instance to calculate 32-bit FNV-1a hash of ASCII string "some string":

```csharp
using HashDepot;
var buffer = Encoding.ASCII.GetBytes("some string");
uint result = Fnv1a.Hash32(buffer); // 32-bit hash
ulong result = Fnv1a.Hash64(buffer); // 64-bit hash
```
  
I started out creating a full blown `HashAlgorithm` implementation first but it seemed more 
suitable for cryptographic hash algorithms. FNV-series are more oriented towards hashing 
simple data, like ASCII strings. So I kept them as static functions.

# Benchmarks

CPU: Intel Core i7-8700 @ 3.20Ghz
1000 iterations over 1048576 bytes of buffer

Name                     | Ops/sec
-------------------------|---------------------------
Checksum (32-bit)        |    2983.43
Fnv1a (32-bit)           |     987.43
Fnv1a (64-bit)           |     997.95
MurmurHash3x86 (32-bit)  |    3605.03
SipHash24 (64-bit)       |    2349.56
xxHash (32-bit)          |    6044.15
xxHash (64-bit)          |    6377.54

# License
MIT License. See LICENSE file for details
