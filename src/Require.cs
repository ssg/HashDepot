using System;
using System.Runtime.CompilerServices;

namespace HashDepot
{
    internal static class Require
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T parameter, string name) where T : class
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}