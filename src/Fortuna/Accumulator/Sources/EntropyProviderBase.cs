using System;
using Fortuna.Accumulator.Event;

namespace Fortuna.Accumulator.Sources
{
    public abstract class EntropyProviderBase : IEntropyProvider
    {
        public abstract string SourceName { get; }
        protected abstract TimeSpan ScheduledPeriod { get; }
        
        protected internal abstract byte[] GetEntropy();

        public virtual IScheduledEvent GetScheduledEvent()
        {
            return new ScheduledEvent
            {
                ScheduledPeriod = ScheduledPeriod,
                EventCallback = GetEntropy
            };
        }

    }
}
