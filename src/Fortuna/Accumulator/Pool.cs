using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace Fortuna.Accumulator
{
    public sealed class Pool : IDisposable
    {
        private readonly SHA256 _sha256 = SHA256.Create();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private long _runningSize;
        public long Size => Interlocked.Read(ref _runningSize);
        
        private byte[] _hash;
        public byte[] Hash
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _hash;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            private set
            {
                // We guarantee this is only ever called from AddEventData, 
                // so we're protected by the outer write lock scope
                _hash = value;
            }
        }

        public void AddEventData(int source, byte[] data)
        {
            if (data.Length > 32) throw new ArgumentOutOfRangeException(nameof(data), "Length must not exceed 32 bytes.");
            if (source < 0 || source > 255) throw new ArgumentOutOfRangeException(nameof(source), "Source identifier must be between 0 and 255 inclusive.");

            _lock.EnterWriteLock();
            try
            {
                var hashData = new[] { (byte) source, (byte) data.Length }.Concat(data).ToArray();
                Hash = _sha256.ComputeHash(hashData);
                Interlocked.Add(ref _runningSize, hashData.Length);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public byte[] ReadFromPool()
        {
            _lock.EnterReadLock();
            try
            {
                Interlocked.Exchange(ref _runningSize, 0);
                return Hash;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #region IDisposable Implementation

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed) return;

            _sha256?.Dispose();
            _lock?.Dispose();

            _isDisposed = true;
        }

        #endregion
    }
}
