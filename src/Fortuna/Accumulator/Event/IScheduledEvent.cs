using System;

namespace Fortuna.Accumulator.Event
{
    public interface IScheduledEvent
    {
        TimeSpan ScheduledPeriod { get; }
        Func<byte[]> EventCallback { get; }
    }
}
