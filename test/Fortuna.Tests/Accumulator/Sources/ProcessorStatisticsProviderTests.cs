using Fortuna.Accumulator.Sources;
using Xunit;

namespace Fortuna.Tests.Accumulator.Sources
{
    public class ProcessorStatisticsProviderTests : EntropyProviderTestBase<ProcessorStatisticsProvider>
    {
        protected override int ExpectedByteCount => 8;


        public ProcessorStatisticsProviderTests() 
            : base(new ProcessorStatisticsProvider())
        {
        }


    }
}
