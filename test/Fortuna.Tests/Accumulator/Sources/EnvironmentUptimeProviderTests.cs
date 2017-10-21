using Fortuna.Accumulator.Sources;

namespace Fortuna.Tests.Accumulator.Sources
{
    public class EnvironmentUptimeProviderTests : EntropyProviderTestBase<EnvironmentUptimeProvider>
    {
        public EnvironmentUptimeProviderTests() 
            : base(new EnvironmentUptimeProvider())
        {
        }

        protected override int ExpectedByteCount => 2;
    }
}
