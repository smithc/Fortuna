using System;
using System.Linq;
using Fortuna.Extensions;
using Xunit;

namespace Fortuna.Tests
{
    [Trait("Type", "Integration")]
    public class PRNGFortunaProviderTests
    {
        private readonly PRNGFortunaProvider _sut;

        public PRNGFortunaProviderTests()
        {
            _sut = PRNGFortunaProviderFactory.Create() as PRNGFortunaProvider;
        }

        [Fact]
        public void ProviderInitializesCorrectly_ReturnsData()
        {
            var data = new byte[1024];

            _sut.GetBytes(data);

            Assert.False(data.SequenceEqual(Enumerable.Repeat(new byte(), 1024)));
        }

        [Fact]
        public void UniformRandomDistributionTest()
        {
            const int numIterations = 100000;
            const int range = 100;
            var distribution = new int[range];

            var data = new byte[sizeof(int)];
            for (int i = 0; i < numIterations; i++)
            {
                _sut.GetBytes(data);

                var value = BitConverter.ToUInt32(data, 0) % range;
                distribution[value]++;
            }

            var expectedValue = numIterations / range;
            var stdDev = 0.1 * expectedValue;

            Assert.True(distribution.All(i => i >= expectedValue - stdDev && i <= expectedValue + stdDev));
        }

        [Fact]
        public void UniformRandomDistribution_WithRandomNumberMethod_Test()
        {
            const int numIterations = 100000;
            const int range = 100;
            var distribution = new int[range];

            for (int i = 0; i < numIterations; i++)
            {
                var value = _sut.RandomNumber(range);
                distribution[value]++;
            }

            var expectedValue = numIterations / range;
            var stdDev = 0.1 * expectedValue;

            Assert.True(distribution.All(i => i >= expectedValue - stdDev && i <= expectedValue + stdDev));
        }

        [Fact]
        public void DisposeProvider_DoesNotThrow()
        {
            _sut.Dispose();
        }

        [Fact]
        public void DisposeProvider_MultipleTimes_DoesNotThrow()
        {
            _sut.Dispose();
            _sut.Dispose();
        }
    }
}
