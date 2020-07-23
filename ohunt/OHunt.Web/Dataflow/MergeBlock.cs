using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OHunt.Web.Dataflow
{
    /// <summary>
    /// Merge some source block to one block
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MergeBlock<T> : ISourceBlock<T>
    {
        private readonly BufferBlock<T> _target;

        private volatile int _totalCount;

        public ILogger Logger { get; set; } = NullLogger.Instance;

        public MergeBlock(IReadOnlyCollection<ISourceBlock<T>> sources)
            : this(sources, new BufferBlock<T>())
        {
        }

        public MergeBlock(IReadOnlyCollection<ISourceBlock<T>> sources, DataflowBlockOptions options)
            : this(sources, new BufferBlock<T>(options))
        {
        }

        private MergeBlock(IReadOnlyCollection<ISourceBlock<T>> sources, BufferBlock<T> target)
        {
            _target = target;
            _totalCount = sources.Count;

            foreach (var source in sources)
            {
                source.LinkTo(_target);

                if (source.Completion.IsCompleted)
                {
                    HandleSourceTaskComplete(source.Completion);
                }
                else
                {
                    source.Completion.ContinueWith(HandleSourceTaskComplete);
                }
            }
        }

        private void HandleSourceTaskComplete(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                Logger.LogDebug("one task complete, current total count {0}", _totalCount);
                if (Interlocked.Decrement(ref _totalCount) <= 0)
                {
                    Logger.LogDebug("All task complete");
                    Complete();
                }
            }
            else
            {
                Logger.LogDebug("task fault");
                ((IDataflowBlock) _target).Fault(
                    task.Exception ?? new Exception(
                        "The source is completed unsuccessfully without an exception"));
            }
        }

        #region dispatched target methods

        public void Complete()
        {
            _target.Complete();
        }

        public void Fault(Exception exception)
        {
            ((ISourceBlock<T>) _target).Fault(exception);
        }

        public Task Completion => _target.Completion;

        public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
        {
            return _target.LinkTo(target, linkOptions);
        }

        public T ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
        {
            return ((ISourceBlock<T>) _target).ConsumeMessage(messageHeader, target, out messageConsumed);
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            return ((ISourceBlock<T>) _target).ReserveMessage(messageHeader, target);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            ((ISourceBlock<T>) _target).ReleaseReservation(messageHeader, target);
        }

        #endregion
    }
}
