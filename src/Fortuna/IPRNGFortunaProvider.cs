using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fortuna
{
    public interface IPRNGFortunaProvider : IDisposable
    {
        void InitializePRNG(CancellationToken token = default(CancellationToken));
        Task InitializePRNGAsync(CancellationToken token = default(CancellationToken));
        void GetBytes(byte[] data);
    }
}
