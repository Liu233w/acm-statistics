using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using OHunt.Web.Dataflow;
using Xunit;

namespace OHunt.Tests.Dataflow
{
    public class DataCachingBlockTests
    {
        private class Int
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
    }
}
