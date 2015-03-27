# fnv
A straightforward implementation of FNV-1 and FNV-1a hash algorithm for .NET. Usage is very simple. For instance to calculate 32-bit FNV-1a hash of ASCII string "some string":

    using Fnv;
    var buffer = Encoding.ASCII.GetBytes("some string");
    uint result = Fnv1a.Hash32(buffer);
  
There is a Nuget package called `Fnv` for your convenience.

# technical notes
I started out creating a full blown `HashAlgorithm` implementation first but it seemed more suitable for cryptographic hash algorithms. FNV-series are more oriented towards hashing simple data, like ASCII strings. So I kept them as static functions.

I wanted this to be well tested (and wanted to give [xUnit.net](https://xunit.github.io/) a shot) so I adapted the [original test vectors](http://www.isthe.com/chongo/src/fnv/test_fnv.c) provided by the creators the algorithm: Glenn Fowler, Landon Curt Noll and Phong Vo, hence the name FNV.

# license
MIT License. See LICENSE file for details
