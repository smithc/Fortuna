using Fortuna.Accumulator.Event;
using System;

namespace Fortuna.Accumulator
{
    public interface IEventScheduler : IDisposable
    {
        event EntropyAvailableHandler EntropyAvailable;

        void ScheduleEvent(int source, IScheduledEvent @event);
    }

    public delegate void EntropyAvailableHandler(int eventSource, byte[] entropy);
}