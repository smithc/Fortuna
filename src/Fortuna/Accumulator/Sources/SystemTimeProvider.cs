using System;
using System.Collections.Generic;
using System.Linq;

namespace Fortuna.Accumulator.Sources
{
    public class SystemTimeProvider : EntropyProviderBase
    {
        public override string SourceName => nameof(SystemTimeProvider);
        protected override TimeSpan ScheduledPeriod => TimeSpan.FromMilliseconds(5);

        protected internal override byte[] GetEntropy()
        {
            IEnumerable<byte> byteTime = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());

            if (!BitConverter.IsLittleEndian)
            {
                byteTime = byteTime.Reverse();
            }

            return byteTime.Take(2).ToArray();
        }
    }
}
