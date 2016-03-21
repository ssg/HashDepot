/// This is for debugging the code in release mode without
/// test framework (NCrunch) instrumentation, so I can see generated
/// assembly code easily.

using HashDepot;

namespace HashConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            byte[] key = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            byte[] buffer = new byte[1500];
            for (int n = 0; n < buffer.Length; n++)
            {
                buffer[n] = (byte)n;
            }
            var result = SipHash.Hash64(buffer, key);
        }
    }
}