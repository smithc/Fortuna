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
        protected override byte[] GetEntropy()
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
                timeBytes = timeBytes.Reverse().Take(2);
                vMemoryBytes = vMemoryBytes.Reverse().Take(2);
                pagedBytes = pagedBytes.Reverse().Take(2);
                workingBytes = workingBytes.Reverse().Take(2);
            }

            return timeBytes
                .Concat(vMemoryBytes)
                .Concat(pagedBytes)
                .Concat(workingBytes)
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
            if (_isDisposed) return;

            _currentProcess?.Dispose();

            _isDisposed = true;
        }

        #endregion
    }
}
