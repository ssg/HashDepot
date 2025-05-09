// <copyright file="Fnv1a.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace HashDepot;

/// <summary>
/// FNV-1a Hash functions.
/// </summary>
public static class Fnv1a
{
    const uint offsetBasis32 = 2166136261;
    const uint prime32 = 16777619;

    const ulong offsetBasis64 = 14695981039346656037;
    const ulong prime64 = 1099511628211;

    /// <summary>
    /// Calculate 32-bit FNV-1a hash value.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <returns>Hash value.</returns>
    public static uint Hash32(Stream stream)
    {
        uint result = offsetBasis32;
        int b;
        while ((b = stream.ReadByte()) >= 0)
        {
            result = prime32 * (result ^ (uint)b);
        }

        return result;
    }

    /// <summary>
    /// Calculate 32-bit FNV-1 hash value.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous hash operation.</returns>
    public static async Task<uint> Hash32Async(Stream stream)
    {
        const int bufferSize = 4096;
        uint result = offsetBasis32;
        var buffer = new byte[bufferSize];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                result = prime32 * (result ^ buffer[i]);
            }
        }

        return result;
    }

    /// <summary>
    /// Calculate 32-bit FNV-1a hash value.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <returns>Hash value.</returns>
    public static uint Hash32(ReadOnlySpan<byte> buffer)
    {
        uint result = offsetBasis32;
        foreach (var b in buffer)
        {
            result = prime32 * (result ^ b);
        }

        return result;
    }

    /// <summary>
    /// Calculate 64-bit FNV-1a hash value.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <returns>Hash value.</returns>
    public static ulong Hash64(Stream stream)
    {
        ulong result = offsetBasis64;
        int b;
        while ((b = stream.ReadByte()) >= 0)
        {
            result = prime64 * (result ^ (uint)b);
        }

        return result;
    }

    /// <summary>
    /// Calculate 64-bit FNV-1 hash value.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous hash operation.</returns>
    public static async Task<ulong> Hash64Async(Stream stream)
    {
        const int bufferSize = 4096;

        ulong result = offsetBasis64;
        var buffer = new byte[bufferSize];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                result = prime64 * (result ^ (uint)buffer[i]);
            }
        }

        return result;
    }

    /// <summary>
    /// Calculate 64-bit FNV-1a hash value.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <returns>Hash value.</returns>
    public static ulong Hash64(ReadOnlySpan<byte> buffer)
    {
        ulong result = offsetBasis64;
        foreach (var b in buffer)
        {
            result = prime64 * (result ^ b);
        }

        return result;
    }
}