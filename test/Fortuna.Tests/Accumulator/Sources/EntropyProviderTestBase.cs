using Fortuna.Accumulator.Sources;
using Xunit;

namespace Fortuna.Tests.Accumulator.Sources
{
    public abstract class EntropyProviderTestBase<TProvider> where TProvider : EntropyProviderBase
    {

        protected readonly TProvider Provider;

        protected abstract int ExpectedByteCount { get; }

        protected EntropyProviderTestBase(TProvider provider)
        {
            Provider = provider;
        }

        [Fact]
        public void GetEntropy_ReturnsCorrectByteCount()
        {
            var bytes = Provider.GetEntropy();

            Assert.NotNull(bytes);
            Assert.Equal(ExpectedByteCount, bytes.Length);
        }

        [Fact]
        public void GetEntropy_ReturnsNoMoreThan_32Bytes()
        {
            var bytes = Provider.GetEntropy();

            Assert.NotNull(bytes);
            Assert.InRange(bytes.Length, 0, 32);
        }
    }
}
