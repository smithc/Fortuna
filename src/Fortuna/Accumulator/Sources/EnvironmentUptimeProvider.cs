using System;
using System.Collections.Generic;
using System.Linq;

namespace Fortuna.Accumulator.Sources
{
    public class EnvironmentUptimeProvider : EntropyProviderBase
    {
        public override string SourceName => "Environment Uptime";
        protected override TimeSpan ScheduledPeriod => TimeSpan.FromMilliseconds(5);
        protected override byte[] GetEntropy()
        {
            IEnumerable<byte> bytes = BitConverter.GetBytes(Environment.TickCount);

            if (!BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse();
            }

            return bytes.Take(2).ToArray();
        }
    }
}
