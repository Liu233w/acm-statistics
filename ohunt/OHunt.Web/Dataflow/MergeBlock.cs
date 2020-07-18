using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OHunt.Web.Dataflow
{
    public class MergeBlock<T> : ISourceBlock<T>
    {
        private readonly List<ISourceBlock<T>> _sources;
        private readonly BufferBlock<T> _target;

        public MergeBlock(List<ISourceBlock<T>> sources)
        {
            _sources = sources;
            _target = new BufferBlock<T>();
        }

        public MergeBlock(List<ISourceBlock<T>> sources, DataflowBlockOptions options)
        {
            _sources = sources;
            _target = new BufferBlock<T>(options);
        }

        public void Complete()
        {
            _target.Complete();
        }

        public void Fault(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task Completion { get; }
        public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
        {
            throw new NotImplementedException();
        }

        public T ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
        {
            throw new NotImplementedException();
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotImplementedException();
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotImplementedException();
        }
    }
}
