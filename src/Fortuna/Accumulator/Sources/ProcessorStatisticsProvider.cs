using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fortuna.Accumulator.Sources
{
    public class ProcessorStatisticsProvider : EntropyProviderBase, IDisposable
    {
        private readonly Process _currentProcess = Process.GetCurrentProcess();

        public override string SourceName => "Current Processor Time";
        protected override TimeSpan ScheduledPeriod => TimeSpan.FromMilliseconds(10);
        protected internal override byte[] GetEntropy()
        {
            var ticks = _currentProcess.TotalProcessorTime.Ticks;
            var vMemory = _currentProcess.VirtualMemorySize64;
            var pagedMemory = _currentProcess.PagedMemorySize64;
            var workingMemory = _currentProcess.WorkingSet64;
            IEnumerable<byte> timeBytes = BitConverter.GetBytes(ticks);
            IEnumerable<byte> vMemoryBytes = BitConverter.GetBytes(vMemory);
            IEnumerable<byte> pagedBytes = BitConverter.GetBytes(pagedMemory);
            IEnumerable<byte> workingBytes = BitConverter.GetBytes(workingMemory);

            if (!BitConverter.IsLittleEndian)
            {
                timeBytes = timeBytes.Reverse();
                vMemoryBytes = vMemoryBytes.Reverse();
                pagedBytes = pagedBytes.Reverse();
                workingBytes = workingBytes.Reverse();
            }

            return timeBytes.Take(2)
                .Concat(vMemoryBytes.Take(2))
                .Concat(pagedBytes.Take(2))
                .Concat(workingBytes.Take(2))
                .ToArray();
        }

        #region IDisposable Implementation

        private bool _isDisposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_isDisposed) return;

            _currentProcess?.Dispose();

            _isDisposed = true;
        }

        #endregion
    }
}
