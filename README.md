# HashDepot
I seem to be implementing certain hash functions that are absent in .NET framework. I decided to converge them into a library. My goals are to have well-tested and Nuget package name is `HashDepot`. Currently supports FNV1, FNV1a and SipHash.

## FNV
A straightforward implementation of FNV-1 and FNV-1a hash algorithm for .NET. Usage is very simple. For instance to calculate 32-bit FNV-1a hash of ASCII string "some string":

    using HashDepot;
    var buffer = Encoding.ASCII.GetBytes("some string");
    uint result = Fnv1a.Hash32(buffer); // 32-bit hash
    ulong result = Fnv1a.Hash64(buffer); // 64-bit hash
  
I started out creating a full blown `HashAlgorithm` implementation first but it seemed more suitable for cryptographic hash algorithms. FNV-series are more oriented towards hashing simple data, like ASCII strings. So I kept them as static functions.

## SipHash
SipHash is resistant to hash-flood attacks against hashtables and uses
a key parameter to ensure HMAC-like authenticity yet faster. Unfortuantely a native .NET implementation does not exist. It is my take on it, and it is really fast for a managed environment. It's standard SipHash-2-4 implementation with 64-bit. To use it:

    using HashDepot;
    var buffer = Encoding.ASCII.GetBytes("some string");
    var key = new byte[16] { .. your random key here .. };
    ulong result = SipHash.Hash64(buffer, key);
    
Note: The largest buffer size supported for hashing is 2GB. Streaming
hashing is not supported.

# License
MIT License. See LICENSE file for details
