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
        public static ulong PartialBytesToUInt64(byte[] buffer, int offset, int leftBytes)
        {
            // a switch/case approach is slightly faster than the loop but .net
            // refuses to inline it due to larger code size.
            ulong result = 0;
            // trying to modify leftBytes would invalidate inlining
            // need to use local variable instead
            for (int i = leftBytes - 1; i >= 0; --i)
            {
                result |= ((ulong)buffer[offset + i]) << (i << 3);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PartialBytesToUInt32(byte[] buffer, int offset, int leftBytes)
        {
            // a switch/case approach is slightly faster than the loop but .net
            // refuses to inline it due to larger code size.
            uint result = 0;
            // trying to modify leftBytes would invalidate inlining
            // need to use local variable instead
            for (int i = leftBytes - 1; i >= 0; --i)
            {
                result |= ((uint)buffer[offset + i]) << (i << 3);
            }
            return result;
        }
    }
}