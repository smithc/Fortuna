using System;

namespace Fortuna.Accumulator.Event
{
    public class ScheduledEvent : IScheduledEvent
    {
        public TimeSpan ScheduledPeriod { get; set; }
        public Func<byte[]> EventCallback { get; set; }
    }
}
