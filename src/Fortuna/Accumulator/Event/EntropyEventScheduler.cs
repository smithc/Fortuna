using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fortuna.Accumulator.Event
{
    public class EntropyEventScheduler : IEventScheduler
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public event EntropyAvailableHandler EntropyAvailable;

        public void ScheduleEvent(int source, IScheduledEvent @event)
        {
            var token = _cancellationTokenSource.Token;

            // The resolution of our scheduler (Task.Delay) is approximately 15ms (on Windows), which should be sufficient for our purposes
            Task.Delay(@event.ScheduledPeriod, token)
                .ContinueWith(t => RaiseEvent(source, @event), token)
                .ContinueWith(t => ScheduleEvent(source, @event), token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
        }

        // Including the 'source' value as an argument here is an explicit design decision.
        // Section 9.5.3.1 specifies that this should be passed at the entropy provider level for security reasons, but because all sources are
        // to be used from within the same Application Domain (and hence same shared memory), this is an acceptable risk.
        private void RaiseEvent(int source, IScheduledEvent @event)
        {
            EntropyAvailable?.Invoke(source, @event.EventCallback());
        }

        #region IDisposable Implementation
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            
            if (disposing)
            {
                _cancellationTokenSource.Cancel(true);
                _cancellationTokenSource.Dispose();
            }

            EntropyAvailable = null;

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
