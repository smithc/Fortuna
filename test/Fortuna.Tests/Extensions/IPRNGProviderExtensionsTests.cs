using System;
using System.Collections.Generic;
using System.Linq;
using Fortuna.Extensions;
using NSubstitute;
using Xunit;

namespace Fortuna.Tests.Extensions
{
    public class IPRNGProviderExtensionsTests
    {
        private readonly IPRNGFortunaProvider _provider;

        public IPRNGProviderExtensionsTests()
        {
            _provider = Substitute.For<IPRNGFortunaProvider>();
            var rng = new Random();

            _provider.WhenForAnyArgs(c => c.GetBytes(null)).Do(c => rng.NextBytes(c.Arg<byte[]>()));
        }

        [Fact]
        public void RandomNumber_UpperBound0_Returnts0()
        {
            var rand = _provider.RandomNumber(0);

            Assert.Equal(0, rand);
        }

        [Theory]
        [MemberData("UpperBounds")]
        public void RandomNumber_UpperBoundN_ReturnsBetween0AndN(int upperBound)
        {
            var rand = _provider.RandomNumber(upperBound);

            Assert.True(rand >= 0, $"random number should be equal to or greater than 0, but was {rand}");
            Assert.True(rand < upperBound, $"random number should be less than upperBound, but was {rand}");
        }


        public static IEnumerable<object[]> UpperBounds()
        {
            return Enumerable.Range(1, 11).Select(i => new object[] { i });
        }

        [Fact, Trait("Category", "Integration")]
        public void RandomNumber_UniformDistribution()
        {
            const int numIterations = 10000;
            const int range = 10;
            var distribution = new int[range];

            for (int i = 0; i < numIterations; i++)
            {
                var value = _provider.RandomNumber(range);
                distribution[value]++;
            }

            var expectedValue = numIterations / range;
            var stdDev = 0.1 * expectedValue;

            Assert.True(distribution.All(i => i >= expectedValue - stdDev && i <= expectedValue + stdDev));
        }
    }
}
