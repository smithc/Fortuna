using System;
using System.Security.Cryptography;

namespace Fortuna.Accumulator.Sources
{
    public class CryptoServiceProvider : EntropyProviderBase, IDisposable
    {
        private readonly RandomNumberGenerator _cryptoService = RandomNumberGenerator.Create();

        public override string SourceName => ".NET RNGCryptoServiceProvider";
        protected override TimeSpan ScheduledPeriod => TimeSpan.FromMilliseconds(100);
        protected override byte[] GetEntropy()
        {
            var randomData = new byte[32];
            _cryptoService.GetBytes(randomData);

            return randomData;
        }

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed) return;

            _cryptoService?.Dispose();

            _isDisposed = true;
        }
    }
}
