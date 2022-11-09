// <copyright file="StreamHelper.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2022 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace HashDepot;

internal static class StreamHelper
{
    /// <summary>
    /// Platform agnostic ReadAsync implementation.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="buffer">Buffer to read data into.</param>
    /// <returns>Read number of bytes.</returns>
    public static async Task<int> ReadAsync(this Stream stream, byte[] buffer)
    {
#if NETSTANDARD
        return await stream.ReadAsync(buffer, 0, buffer.Length);
#else
        return await stream.ReadAsync(buffer.AsMemory());
#endif
    }
}
