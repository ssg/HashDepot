using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HashDepot
{
    internal static class Bits
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int bits)
        {
            return (value << bits) | (value >> (64 - bits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(uint value, int bits)
        {
            return (value >> bits) | (value << (32 - bits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong PartialBytesToUInt64(byte* ptr, int leftBytes)
        {
            // a switch/case approach is slightly faster than the loop but .net
            // refuses to inline it due to larger code size.
            ulong result = 0;
            // trying to modify leftBytes would invalidate inlining
            // need to use local variable instead
            for (int i = 0; i < leftBytes; i++)
            {
                result |= ((ulong)*ptr++) << (i << 3);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint PartialBytesToUInt32(byte* ptr, int leftBytes)
        {
            Debug.Assert(leftBytes > 0 && leftBytes < 4);
            // a switch/case approach is slightly faster than the loop but .net
            // refuses to inline it due to larger code size.
            uint result = *ptr++;
            if (leftBytes > 1)
            {
                result |= (uint)(*ptr++ << 8);
            }
            if (leftBytes > 2)
            {
                result |= (uint)(*ptr++ << 16);
            }
            return result;
        }
    }
}