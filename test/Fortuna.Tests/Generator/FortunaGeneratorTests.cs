using System;
using System.Linq;
using Fortuna.Generator;
using Xunit;
using Xunit.Abstractions;

namespace Fortuna.Tests.Generator
{
    public class FortunaGeneratorTests
    {
        private FortunaGenerator _sut;
        private readonly ITestOutputHelper _output;

        public FortunaGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
            _sut = new FortunaGenerator();
        }

        [Fact]
        public void GetBytes_ReturnsData()
        {
            var emptyArray = new byte[1024];
            var data = new byte[1024];

            Assert.True(emptyArray.SequenceEqual(data));

            _sut.GenerateBytes(data);

            Assert.False(emptyArray.SequenceEqual(data));
        }

        [Fact]
        public void GetBytes_WithSameSeed_ReturnsSameData()
        {
            var seed = new byte[1];

            _sut = new FortunaGenerator(seed);

            var data = new byte[1024];
            _sut.GenerateBytes(data);

            _sut = new FortunaGenerator(seed);

            var data2 = new byte[1024];
            _sut.GenerateBytes(data2);

            Assert.True(data.SequenceEqual(data2));
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
                _sut.GenerateBytes(data);

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
    }
}
