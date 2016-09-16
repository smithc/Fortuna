using Fortuna.Accumulator.Sources;

namespace Fortuna.Tests.Accumulator.Sources
{
    public class SystemTimeProviderTests : EntropyProviderTestBase<SystemTimeProvider>
    {
        public SystemTimeProviderTests() 
            : base(new SystemTimeProvider())
        {
        }

        protected override int ExpectedByteCount => 2;
    }
}
