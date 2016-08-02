using System;

namespace Fortuna.Accumulator.Sources
{
    public class GarbageCollectionProvider : EntropyProviderBase
    {
        public override string SourceName => ".NET Garbage Collector";
        protected override TimeSpan ScheduledPeriod => TimeSpan.FromMilliseconds(50);
        protected override byte[] GetEntropy()
        {
            var totalMemory = GC.GetTotalMemory(false);
            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);

            var divisor = Math.Min(1, gen0 + gen1 + gen2);

            return BitConverter.GetBytes(totalMemory % divisor);
        }
    }
}
