using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Fortuna.Accumulator;
using Fortuna.Accumulator.Event;
using Fortuna.Accumulator.Sources;
using Fortuna.Generator;
using Fortuna.Initialization;

namespace Fortuna
{
    public static class PRNGFortunaProviderFactory
    {
        public static IPRNGFortunaProvider Create(CancellationToken token = default(CancellationToken))
        {
            var prng = GetProvider();
            prng.InitializePRNG(token);

            return prng;
        }

        public static async Task<IPRNGFortunaProvider> CreateAsync(CancellationToken token = default(CancellationToken))
        {
            var prng = GetProvider();
            await prng.InitializePRNGAsync(token).ConfigureAwait(false);

            return prng;
        }

        public static IPRNGFortunaProvider CreateWithSeedFile(Stream seedStream,
            CancellationToken token = default(CancellationToken))
        {
            var prng = GetSeedFileDecorator(seedStream);
            prng.InitializePRNG(token);

            return prng;
        }

        public static async Task<IPRNGFortunaProvider> CreateWithSeedFileAsync(Stream seedStream,
            CancellationToken token = default(CancellationToken))
        {
            var prng = GetSeedFileDecorator(seedStream);
            await prng.InitializePRNGAsync(token).ConfigureAwait(false);

            return prng;
        }

        private static PRNGFortunaProvider GetProvider()
        {
            var providers = new IEntropyProvider[]
            {
                new SystemTimeProvider(),
                new GarbageCollectionProvider(),
                new CryptoServiceProvider(),
                new EnvironmentUptimeProvider(),
                new ProcessorStatisticsProvider()
            };

            return new PRNGFortunaProvider(
                new FortunaGenerator(),
                new FortunaAccumulator(new EntropyEventScheduler(), providers));
        }

        private static IPRNGFortunaProvider GetSeedFileDecorator(Stream seedStream)
        {
            return new SeedFileDecorator(GetProvider(), seedStream);
        }
    }
}
