# HashDepot
A library to provide well-tested various hash algorithm implementations. 
Nuget package name is `HashDepot`. Currently supports FNV1, FNV1a and SipHash.

## FNV
A straightforward implementation of FNV-1 and FNV-1a hash algorithm for .NET. Usage is very simple. For instance to calculate 32-bit FNV-1a hash of ASCII string "some string":

    using Fnv;
    var buffer = Encoding.ASCII.GetBytes("some string");
    uint result = Fnv1a.Hash32(buffer);
  
I started out creating a full blown `HashAlgorithm` implementation first but it seemed more suitable for cryptographic hash algorithms. FNV-series are more oriented towards hashing simple data, like ASCII strings. So I kept them as static functions.

## SipHash
SipHash is resistant to hash-flood attacks against hashtables. Unfortuantely a native .NET
implementation does not exist. It is my take on it, and it is really fast for a managed
environment. 

# License
MIT License. See LICENSE file for details
