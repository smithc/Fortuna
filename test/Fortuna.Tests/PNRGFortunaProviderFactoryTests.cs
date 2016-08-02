using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fortuna.Tests
{
    [Trait("Type", "Integration")]
    public class PRNGFortunaProviderFactoryTests
    {
        [Fact]
        public void Create_Cancelled_ThrowsCancellationException()
        {
            // Create an immediate cancellation
            var source = new CancellationTokenSource();
            source.Cancel();

            Assert.Throws<OperationCanceledException>(() => PRNGFortunaProviderFactory.Create(token: source.Token));
        }

        [Fact]
        public async Task CreateAsync_Cancelled_ThrowsCancellationException()
        {
            // Create an immediate cancellation
            var source = new CancellationTokenSource();
            source.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(async () => await PRNGFortunaProviderFactory.CreateAsync(token: source.Token));
        }

        [Fact, Trait("Category", "Integration")]
        public void Create_WithSeedFile_DoesNotThrow()
        {
            var memoryStream = new MemoryStream(new byte[64]);
            PRNGFortunaProviderFactory.CreateWithSeedFile(memoryStream);
        }
    }
}
