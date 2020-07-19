using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using OHunt.Web.Dataflow;
using Xunit;

namespace OHunt.Tests.Dataflow
{
    public class MergeBlockTests
    {
        [Fact]
        public void It_ShouldMergeDataFromMultipleSources()
        {
            // arrange
            var b1 = new BufferBlock<int>();
            var b2 = new BufferBlock<int>();
            var b3 = new BufferBlock<int>();

            var mergeBlock = new MergeBlock<int>(new[] { b1, b2, b3 });

            // act
            b1.Post(1);
            b2.Post(2);
            b3.Post(3);
            b1.Post(1);

            const int cnt = 4;
            int[] bf = new int[4];
            for (int i = 0; i < cnt; i++)
            {
                bf[i] = mergeBlock.Receive();
            }

            // assert
            bf.Should().BeEquivalentTo(new[] { 1, 1, 2, 3 });
        }

        [Fact]
        public async Task WhenAnySourcesFault_ItShouldFault()
        {
            // arrange
            var b1 = new BufferBlock<int>();
            var b2 = new BufferBlock<int>();
            var mergeBlock = new MergeBlock<int>(new[] { b1, b2 });

            // act
            ((ISourceBlock<int>) b1).Fault(new Exception("a fault"));

            await Task.Delay(TimeSpan.FromSeconds(1));

            // assert
            await mergeBlock.Completion
                .ShouldResult().ThrowAsync<Exception>()
                .WithMessage("a fault");
        }

        [Fact]
        public async Task WhenAllSourcesComplete_ItShouldComplete()
        {
            // arrange
            var b1 = new BufferBlock<int>();
            var b2 = new BufferBlock<int>();
            var b3 = new BufferBlock<int>();

            var mergeBlock = new MergeBlock<int>(new[] { b1, b2, b3 });

            // act
            b1.Post(1);
            b2.Post(2);
            b3.Post(3);
            b1.Post(1);

            b1.Complete();
            b2.Complete();
            b3.Complete();

            await b1.Completion;
            await b2.Completion;
            await b3.Completion;
            
            await mergeBlock.Completion;
            mergeBlock.TryReceiveAll(out var list);

            // assert
            mergeBlock.Completion.IsCompletedSuccessfully.Should().BeTrue();
            list.Should().BeEquivalentTo(new[] { 1, 1, 2, 3 });
        }

        [Fact]
        public async Task WhileSourcesFaultAndCompleteAtTheSameTime_ItShouldFault()
        {
            // arrange
            var b1 = new BufferBlock<int>();
            var b2 = new BufferBlock<int>();
            var mergeBlock = new MergeBlock<int>(new[] { b1, b2 });

            // act
            ((ISourceBlock<int>) b1).Fault(new Exception("a fault"));
            b2.Complete();

            // assert
            await mergeBlock.Completion
                .ShouldResult().ThrowAsync<Exception>()
                .WithMessage("a fault");
        }
    }
}
