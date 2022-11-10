# Changes

## 3.1.0

### Breaking changes
- This version targets .NET 6.0.
- Removed APIs that received `byte[]`.
- Removed all unsafe code. This had slight impact on performance, but
  thanks to the Span related optimizations, it's not as bad as it was in .NET Framework. There's
  still some use of `Unsafe.ReadUnaligned` for some perf improvements, but all that ugly
  pointer arithmetic has gone away for marginal cost for the better.
- Removed .NET Standard 2.0 support (sorry, but new .NET is so nice to code, and maintaining #ifdefs is unnecessarily cumbersome)

### Improvements
- Memory use improved in async functions.
- Benchmark now uses BenchmarkDotNet.

## 3.0.0
This release is obsolete and unlisted. Do not use.

## 2.0.3 and previous versions
Lost to the history. JK, check the releases for the change notes on those.