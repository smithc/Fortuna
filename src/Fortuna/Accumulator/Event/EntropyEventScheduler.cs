using System.Threading.Tasks;

namespace Fortuna.Accumulator.Event
{
    public class EntropyEventScheduler : IEventScheduler
    {
        public event EntropyAvailableHandler EntropyAvailable;

        public void ScheduleEvent(int source, IScheduledEvent @event)
        {
            // The resolution of our scheduler (Task.Delay) is approximately 15ms (on Windows), which should be sufficient for our purposes
            Task.Delay(@event.ScheduledPeriod)
                .ContinueWith(t => RaiseEvent(source, @event))
                .ContinueWith(t => ScheduleEvent(source, @event), TaskContinuationOptions.ExecuteSynchronously);
        }

        // Including the 'source' value as an argument here is an explicit design decision.
        // Section 9.5.3.1 specifies that this should be passed at the entropy provider level for security reasons, but because all sources are
        // to be used from within the same Application Domain (and hence same shared memory), this is an acceptable risk.
        private void RaiseEvent(int source, IScheduledEvent @event)
        {
            EntropyAvailable?.Invoke(source, @event.EventCallback());
        }
    }
}
