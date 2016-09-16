using Fortuna.Accumulator.Sources;

namespace Fortuna.Tests.Accumulator.Sources
{
    public class CryptoServiceProviderTests : EntropyProviderTestBase<CryptoServiceProvider>
    {
        public CryptoServiceProviderTests() 
            : base(new CryptoServiceProvider())
        {
        }

        protected override int ExpectedByteCount => 32;
    }
}
