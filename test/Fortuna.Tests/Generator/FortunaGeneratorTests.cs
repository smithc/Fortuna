using System;
using System.Linq;
using Fortuna.Generator;
using Xunit;

namespace Fortuna.Tests.Generator
{
    public class FortunaGeneratorTests
    {
        private FortunaGenerator _sut;

        public FortunaGeneratorTests()
        {
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
            const int numIterations = 100000;
            const int range = 100;
            var distribution = new int[range];

            var data = new byte[sizeof(int)];
            for (int i = 0; i < numIterations; i++)
            {
                _sut.GenerateBytes(data);

                var value = BitConverter.ToUInt32(data, 0) % range;
                distribution[value]++;
            }

            var expectedValue = numIterations/range;
            var stdDev = 0.1*expectedValue;

            Assert.True(distribution.All(i => i >= expectedValue - stdDev && i <= expectedValue + stdDev));
        }
    }
}
