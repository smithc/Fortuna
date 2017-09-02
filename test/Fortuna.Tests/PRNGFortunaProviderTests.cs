using System;
using System.Linq;
using Fortuna.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Fortuna.Tests
{
    [Trait("Type", "Integration")]
    public class PRNGFortunaProviderTests
    {
        private readonly PRNGFortunaProvider _sut;
        private readonly ITestOutputHelper _output;

        public PRNGFortunaProviderTests(ITestOutputHelper output)
        {
            _output = output;
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
            const int numIterations = 250_000;
            const int range = 1000;
            var distribution = new int[range];

            var data = new byte[sizeof(int)];
            for (int i = 0; i < numIterations; i++)
            {
                _sut.GetBytes(data);

                var value = BitConverter.ToUInt32(data, 0) % range;
                distribution[value]++;
            }

            var expectedValue = numIterations / range;
            var stdDev = range - 1 / Math.Sqrt(12);

            var outliers = distribution
                .Where(i => i < expectedValue - stdDev || i > expectedValue + stdDev)
                .ToArray();
            if (outliers.Any())
            {
                _output.WriteLine($"Expected value: {expectedValue}");
                foreach (var outlier in outliers)
                {
                    var difference = Math.Max(outlier, expectedValue) - Math.Min(outlier, expectedValue);
                    _output.WriteLine($"{outlier}: diff of {difference} ({difference / stdDev}% of std. dev)");
                }
                Assert.True(false, "Outliers detected");
            }
        }

        [Fact]
        public void UniformRandomDistribution_WithRandomNumberMethod_Test()
        {
            const int numIterations = 100_000;
            const int range = 500;
            var distribution = new int[range];

            for (int i = 0; i < numIterations; i++)
            {
                var value = _sut.RandomNumber(range);
                distribution[value]++;
            }

            var expectedValue = numIterations / range;
            var stdDev = range - 1 / Math.Sqrt(12);

            var outliers = distribution
                .Where(i => i < expectedValue - stdDev || i > expectedValue + stdDev)
                .ToArray();
            if (outliers.Any())
            {
                _output.WriteLine($"Expected value: {expectedValue}");
                foreach (var outlier in outliers)
                {
                    var difference = Math.Max(outlier, expectedValue) - Math.Min(outlier, expectedValue);
                    _output.WriteLine($"{outlier}: diff of {difference} ({difference / stdDev}% of std. dev)");
                }
                Assert.True(false, "Outliers detected");
            }
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
