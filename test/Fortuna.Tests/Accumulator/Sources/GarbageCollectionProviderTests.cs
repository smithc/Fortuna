using Fortuna.Accumulator.Sources;

namespace Fortuna.Tests.Accumulator.Sources
{
    public class GarbageCollectionProviderTests : EntropyProviderTestBase<GarbageCollectionProvider>
    {
        public GarbageCollectionProviderTests() 
            : base(new GarbageCollectionProvider())
        {
        }

        protected override int ExpectedByteCount => sizeof(long);
    }
}