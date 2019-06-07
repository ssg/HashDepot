// <copyright file="Require.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2019 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace HashDepot
{
    /// <summary>
    /// Helpers for argument validation.
    /// </summary>
    internal static class Require
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T parameter, string name)
            where T : class
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), "Incorrectly passed a null value as name");
            }

            if (parameter == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}