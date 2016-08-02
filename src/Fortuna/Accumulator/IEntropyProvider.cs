using Fortuna.Accumulator.Event;

namespace Fortuna.Accumulator
{
    public interface IEntropyProvider
    {
        string SourceName { get; }

        IScheduledEvent GetScheduledEvent();
    }
}
