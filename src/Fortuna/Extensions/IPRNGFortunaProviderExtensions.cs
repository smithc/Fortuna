using System;
using System.Linq;

namespace Fortuna.Extensions
{
    public static class IPRNGFortunaProviderExtensions
    {

        /// <summary>
        /// Selects a random number between 0 and an upper-bound
        /// </summary>
        /// <param name="provider">Random Number Generator to use</param>
        /// <param name="upperBound">The upper-bound (exclusive) of random number selection</param>
        /// <returns>A random number between 0 (inclusive) and the given upper-bound (exclusive)</returns>
        public static int RandomNumber(this IPRNGFortunaProvider provider, int upperBound)
        {
            int numBits = PowUpperBound(upperBound);
            var numBytes = (int)Math.Ceiling(numBits / 8d);

            //var offset = numBits % 8;
            var offset = 8 - numBits % (numBytes * 8);
            int retVal;

            var bytes = new byte[numBytes];

            do
            {
                provider.GetBytes(bytes);

                // Now we need to conditionally dispose of any extra bits
                if (offset != 0)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        bytes[bytes.Length - 1] = (byte) (bytes[bytes.Length - 1] << offset);
                        bytes[bytes.Length - 1] = (byte)(bytes[bytes.Length - 1] >> offset);
                    }
                    else
                    {
                        bytes[0] = (byte) (bytes[0] >> offset);
                        bytes[0] = (byte)(bytes[0] << offset);
                    }
                }

                var prepostFix = Enumerable.Repeat((byte) 0, 4 - bytes.Length);
                var intBytes = BitConverter.IsLittleEndian
                    ? bytes.Concat(prepostFix)
                    : prepostFix.Concat(bytes);

                retVal = BitConverter.ToInt32(intBytes.ToArray(), 0);
            } while (retVal != 0 && retVal >= upperBound);

            return Math.Abs(retVal);
        }

        private static int PowUpperBound(int count, int pow = 1)
        {
            if (count < 2) return 1;

            var value = Math.Pow(2, pow);
            if (value >= count) return pow;

            return PowUpperBound(count, pow + 1);
        }

    }
}
