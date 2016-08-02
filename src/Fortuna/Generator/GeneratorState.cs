using System;
using System.Numerics;

namespace Fortuna.Generator
{
    internal struct GeneratorState
    {
        internal byte[] Key;
        internal BigInteger Counter;

        internal byte[] CounterBytes
        {
            get
            {
                var bytes = new byte[16];
                var counterBytes = Counter.ToByteArray();

                Array.Copy(counterBytes, bytes, counterBytes.Length);

                return bytes;
            }
        }
    }
}
