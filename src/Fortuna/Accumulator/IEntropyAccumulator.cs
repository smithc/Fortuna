using System;

namespace Fortuna.Accumulator
{
    public interface IEntropyAccumulator : IDisposable
    {
        bool HasEnoughEntropy { get; }

        byte[] GetRandomDataFromPools();
    }
}
