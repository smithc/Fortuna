using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fortuna.Accumulator;
using Fortuna.Accumulator.Event;
using Fortuna.Generator;
using Fortuna.Initialization;
using NSubstitute;
using Xunit;

namespace Fortuna.Tests.Initialization
{
    public class SeedFileDecoratorTests
    {
        private readonly IReseedableFortunaProvider _mockProvider;
        private readonly Stream _mockFile;

        public SeedFileDecoratorTests()
        {
            _mockProvider = Substitute.For<IReseedableFortunaProvider>();
            _mockFile = Substitute.For<Stream>();

            _mockFile.CanRead.Returns(true);
            _mockFile.CanWrite.Returns(true);
            _mockFile.CanSeek.Returns(true);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void CreateDecorator_FileHasIncorrectCapabilities_Throws(bool canRead, bool canWrite)
        {
            _mockFile.CanRead.Returns(canRead);
            _mockFile.CanWrite.Returns(canWrite);

            Assert.Throws<ArgumentException>(() => new SeedFileDecorator(_mockProvider, _mockFile));
        }

        [Fact]
        public void CreateDecorator_FileNotUsable_ReseedsFromEntropy()
        {
            var decorator = new SeedFileDecorator(_mockProvider, _mockFile);
            decorator.InitializePRNG();

            _mockProvider.Received(1).InitializePRNG();
            _mockFile.DidNotReceive().Read(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>());
        }

        [Fact]
        public void CreateDecorator_FileHasEntropy_ReseedsFromFile()
        {
            _mockFile.Length.Returns(64);

            var decorator = new SeedFileDecorator(_mockProvider, _mockFile);
            decorator.InitializePRNG();

            _mockProvider.DidNotReceive().InitializePRNG();
            _mockFile.Received().Read(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>());

            // Finally, assert that the new seed was generated and placed in the file
            _mockFile.Received().Seek(0, SeekOrigin.Begin);
            _mockFile.Received().Write(Arg.Any<byte[]>(), 0, 64);
            _mockFile.Received().Flush();
        }

        [Fact]
        public async Task CreateDecoratorAsync_FileHasEntropy_ReseedsFromFile()
        {
            _mockFile.Length.Returns(64);

            var decorator = new SeedFileDecorator(_mockProvider, _mockFile);
            await decorator.InitializePRNGAsync();

            await _mockProvider.DidNotReceive().InitializePRNGAsync();
            await _mockFile.Received().ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());

            // Finally, assert that the new seed was generated and placed in the file
            _mockFile.Received().Seek(0, SeekOrigin.Begin);
            await _mockFile.Received().WriteAsync(Arg.Any<byte[]>(), 0, 64, Arg.Any<CancellationToken>());
            await _mockFile.Received().FlushAsync(Arg.Any<CancellationToken>());
        }

        [Fact, Trait("Type", "Integration")]
        public void CreateDecorator_SeedsFromFile_ReturnsData()
        {
            _mockFile.Length.Returns(64);

            var generator = new FortunaGenerator();
            var accumulator = new FortunaAccumulator(new EntropyEventScheduler(), Enumerable.Empty<IEntropyProvider>());

            var decorator = new SeedFileDecorator(new PRNGFortunaProvider(generator, accumulator), _mockFile);
            decorator.InitializePRNG();

            var data = new byte[1024];
            decorator.GetBytes(data);

            Assert.NotEqual((IEnumerable<byte>) data, new byte[1024]);
        }
    }
}
