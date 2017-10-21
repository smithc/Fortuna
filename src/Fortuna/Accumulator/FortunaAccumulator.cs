using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Fortuna.Accumulator
{
    public sealed class FortunaAccumulator : IEntropyAccumulator
    {
        private const int MaxNumberOfSources = 255;
        private const int NumberOfPools = 32;
        private const int MinEntropyPoolSize = 64;

        private readonly Pool[] _entropyPools = new Pool[NumberOfPools];
        private readonly IEventScheduler _eventScheduler;
        private readonly IReadOnlyCollection<IEntropyProvider> _entropyProviders;

        private int _sourceCount;
        private int _runningCount;
        private int _requestDataCount;

        public bool HasEnoughEntropy => _entropyPools[0].Size > MinEntropyPoolSize;

        public FortunaAccumulator(IEventScheduler eventScheduler, IEnumerable<IEntropyProvider> entropyProviders)
        {
            if (eventScheduler == null) throw new ArgumentNullException(nameof(eventScheduler));
            if (entropyProviders == null) throw new ArgumentNullException(nameof(entropyProviders));
            _eventScheduler = eventScheduler;

            _entropyProviders = new ReadOnlyCollection<IEntropyProvider>(entropyProviders.ToList());

            InitializePools();
            RegisterEntropySources(_entropyProviders);

            _eventScheduler.EntropyAvailable += AccumulateEntropy;
        }

        public byte[] GetRandomDataFromPools()
        {
            if (!HasEnoughEntropy)
                throw new InvalidOperationException("The accumulator does not yet have enough entropy to generate data.");

            const int maxSeedSize = NumberOfPools * 32;

            var requestForDataCount = _requestDataCount++;
            var randomData = new byte[maxSeedSize];

            var bufferIndex = 0;
            for (var poolIndex = 0; poolIndex < NumberOfPools; poolIndex++)
            {
                if (requestForDataCount%Math.Pow(2, poolIndex) != 0)
                {
                    // We can break out the first time we hit this condition
                    break;
                }

                var poolData = _entropyPools[poolIndex].ReadFromPool();

                Debug.Assert(poolData.Length == 32);
                    
                poolData.CopyTo(randomData, bufferIndex);
                bufferIndex += poolData.Length;
            }

            return randomData;
        }

        #region Private Functions

        private void InitializePools()
        {
            for (var i = 0; i < _entropyPools.Length; i++)
            {
                _entropyPools[i] = new Pool();
            }
        }

        private void RegisterEntropySources(IEnumerable<IEntropyProvider> entropyProviders)
        {
            foreach (var entropyProvider in entropyProviders)
            {
                var eventSource = _sourceCount++;
                if (eventSource > MaxNumberOfSources)
                    throw new InvalidOperationException($"Cannot configure more than {MaxNumberOfSources} for accumulating entropy.");

                _eventScheduler.ScheduleEvent(eventSource, entropyProvider.GetScheduledEvent());
            }
        }

        private void AccumulateEntropy(int eventSource, byte[] entropy)
        {
            var runningCount = Interlocked.Increment(ref _runningCount) - 1;
            var poolSize = _entropyPools.Length;

            var poolIndex = runningCount % poolSize;
            _entropyPools[poolIndex].AddEventData(eventSource, entropy);
        }

        #endregion

        #region IDisposable Implementation

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed) return;

            _eventScheduler.EntropyAvailable -= AccumulateEntropy;
            _eventScheduler?.Dispose();

            foreach (var pool in _entropyPools)
            {
                pool?.Dispose();
            }

            foreach (var provider in _entropyProviders.OfType<IDisposable>())
            {
                provider.Dispose();
            }

            _isDisposed = true;
        }

        #endregion
    }
}
