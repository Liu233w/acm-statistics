using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using OHunt.Web.Dataflow;
using Xbehave;
using Xunit;

namespace OHunt.Tests.Dataflow
{
    public class DataCachingBlockTests
    {
        #region prepare

        public class Int
        {
            public Int(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private readonly List<int> _received = new List<int>();
        private readonly IPropagatorBlock<DataCachingMessage<Int>, Int> _block;
        private readonly ActionBlock<Int> _target;

        public DataCachingBlockTests()
        {
            _block = DataCachingBlockFactory.CreateBlock<Int>(10);

            _target = new ActionBlock<Int>(item => { _received.Add(item.Value); });
            _block.LinkTo(_target, new DataflowLinkOptions { PropagateCompletion = true });
        }

        #endregion

        [Fact]
        public async Task It_ShouldWorkCorrectly()
        {
            // act
            _block.Post(Message(1));
            _block.Post(Message(2));
            _block.Post(Message(3));
            _block.Post(DataCachingMessage<Int>.SubmitMessage);

            _block.Complete();

            // assert
            await _target.Completion;
            _received.Should().Equal(1, 2, 3);

            static DataCachingMessage<Int> Message(int value)
            {
                return DataCachingMessage<Int>.OfEntity(new Int(value));
            }
        }

        [Fact]
        public async Task WhenThereIsNoCachedItem_AndReceivingDiscardMessage_ItShouldDoNothing()
        {
            // act
            _block.Post(DataCachingMessage<Int>.DiscardMessage);
            await WaitOutput();

            // assert
            _received.Should().BeEmpty();
        }

        [Fact]
        public async Task WhenThereIsNoCachedItem_AndBlockComplete_ItShouldDoNothing()
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void WhenThereAreCachedItems_AndReceivingDiscardMessage_ItShouldDiscardThem()
        {
            "Given cached items".x(() =>
            {
                _block.Post(Message(1));
                _block.Post(Message(2));
                _block.Post(Message(3));
            });

            "When receiving discard message".x(() => _block.Post(DataCachingMessage<Int>.DiscardMessage));

            "Then items are discarded".x(async () =>
            {
                await WaitOutput();
                _received.Should().BeEmpty();
            });

            "When receiving entity again".x(() =>
            {
                _block.Post(Message(1));
                _block.Post(DataCachingMessage<Int>.SubmitMessage);
            });

            "Then items are available".x(async () =>
            {
                await WaitOutput();
                _received.Should().Equal(1);
            });

            static DataCachingMessage<Int> Message(int value)
            {
                return DataCachingMessage<Int>.OfEntity(new Int(value));
            }
        }

        [Fact]
        public async Task WhenThereAreCachedItems_AndBlockComplete_ItShouldDiscardThem()
        {
            throw new NotImplementedException();
        }

        [Theory]
        [MemberData(nameof(Test_SubmitMessage_Data))]
        public async Task Test_SubmitMessage(int[] cachedItems, DataCachingMessage<Int> message, int[] result)
        {
            foreach (var cachedItem in cachedItems)
            {
                _block.Post(DataCachingMessage<Int>.OfEntity(new Int(cachedItem)));
            }

            _block.Post(message);
            await WaitOutput();

            _received.Should().Equal(result);
        }

        public static IEnumerable<object[]> Test_SubmitMessage_Data()
        {
            yield return Test(
                new[] { 1, 2, 3 },
                DataCachingMessage<Int>.OfEntity(new Int(4)),
                new int[] { });
            yield return Test(
                new int[] { },
                DataCachingMessage<Int>.OfEntity(new Int(4)),
                new int[] { });

            yield return Test(
                new int[] { },
                DataCachingMessage<Int>.OfEntity(new Int(4), true),
                new[] { 4 });
            yield return Test(
                new[] { 1, 2, 3 },
                DataCachingMessage<Int>.OfEntity(new Int(4), true),
                new[] { 1, 2, 3, 4 });
            yield return Test(
                new[] { 1, 2, 3 },
                DataCachingMessage<Int>.SubmitMessage,
                new[] { 1, 2, 3 });

            static object[] Test(int[] cachedItems, DataCachingMessage<Int> message, int[] result)
            {
                return new object[] { cachedItems, message, result };
            }
        }

        [Fact]
        public async Task WhenCacheIsFull_AndReceivingMessage_ItShouldFail()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task WhenComplete_DownstreamShouldComplete()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task WhenFail_DownstreamShouldFail()
        {
            throw new NotImplementedException();
        }

        private async Task WaitOutput()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
