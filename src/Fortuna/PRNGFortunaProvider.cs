using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Fortuna.Accumulator;
using Fortuna.Generator;

namespace Fortuna
{
    public sealed class PRNGFortunaProvider : RandomNumberGenerator, IReseedableFortunaProvider
    {
        private static readonly TimeSpan OneHundredMilliseconds = TimeSpan.FromMilliseconds(100);

        private readonly IGenerator _generator;
        private readonly IEntropyAccumulator _accumulator;

        private DateTime _lastReseedTime = DateTime.MinValue;
        private int _reseedCount;

        public PRNGFortunaProvider(IGenerator generator, IEntropyAccumulator accumulator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));
            _generator = generator;
            _accumulator = accumulator;
        }

        public new IPRNGFortunaProvider Create()
        {
            return PRNGFortunaProviderFactory.Create();
        }

        public void InitializePRNG(CancellationToken token = default(CancellationToken))
        {
            // Setup the PRNG here, and wait for data
            while (!_accumulator.HasEnoughEntropy)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(1);
            }
        }

        public async Task InitializePRNGAsync(CancellationToken token = default(CancellationToken))
        {
            // Setup the PRNG here, and wait for data
            while (!_accumulator.HasEnoughEntropy)
            {
                await Task.Delay(1, token).ConfigureAwait(false);
            }
        }

        public override void GetBytes(byte[] data)
        {
            var timeSinceLastReseed = DateTime.UtcNow - _lastReseedTime;

            if (_accumulator.HasEnoughEntropy && timeSinceLastReseed > OneHundredMilliseconds)
            {
                // Reseed the Generator
                Reseed(_accumulator.GetRandomDataFromPools());
            }

            if (_reseedCount == 0)
            {
                throw new InvalidOperationException("The PRNG has not yet been seeded."); 
            }

            // Any necessary reseeds should have completed - now generate the random data
             _generator.GenerateBytes(data);
        }

        void IReseedableFortunaProvider.Reseed(byte[] seed)
        {
            Reseed(seed);
        }

        private void Reseed(byte[] seed)
        {
            _reseedCount++;
            _generator.Reseed(seed);
            _lastReseedTime = DateTime.UtcNow;
        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            _accumulator?.Dispose();

            base.Dispose(disposing);

            _isDisposed = true;
        }
    }
}
