using Fortuna.Accumulator.Event;

namespace Fortuna.Accumulator
{
    public interface IEventScheduler
    {
        event EntropyAvailableHandler EntropyAvailable;

        void ScheduleEvent(int source, IScheduledEvent @event);
    }

    public delegate void EntropyAvailableHandler(int eventSource, byte[] entropy);
}