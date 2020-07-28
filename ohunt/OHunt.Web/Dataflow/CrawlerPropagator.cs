using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;

namespace OHunt.Web.Dataflow
{
    public class CrawlerPropagator : ITargetBlock<CrawlerMessage>
    {
        private const int BufferCapacity = 1000;

        private readonly ITargetBlock<CrawlerMessage> _input;
        private readonly ITargetBlock<Submission> _submissionOutput;
        private readonly ITargetBlock<CrawlerError> _errorOutput;
        private readonly Queue<CrawlerMessage> _queue;

        public CrawlerPropagator(
            ITargetBlock<Submission> submissionOutput,
            ITargetBlock<CrawlerError> errorOutput)
        {
            _submissionOutput = submissionOutput;
            _errorOutput = errorOutput;

            _input = InitInputBlock();
            _queue = new Queue<CrawlerMessage>(BufferCapacity);

            Completion = _input.Completion.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _submissionOutput.Fault(task.Exception);
                    _errorOutput.Fault(task.Exception);
                }
                else
                {
                    // TODO: handle cancel
                    _submissionOutput.Complete();
                    _errorOutput.Complete();
                }
            });
        }

        private ActionBlock<CrawlerMessage> InitInputBlock()
        {
            return new ActionBlock<CrawlerMessage>(async message =>
            {
                if (message.IsRevertRequested)
                {
                    _queue.Clear();
                    return;
                }

                if (message.Submission != null || message.CrawlerError != null)
                {
                    _queue.Enqueue(message);
                }

                if (message.IsCheckPoint)
                {
                    await Dispatch();
                }
            });
        }

        private async Task Dispatch()
        {
            while (_queue.TryDequeue(out var message))
            {
                if (message.Submission != null)
                {
                    await _submissionOutput.SendAsync(message.Submission);
                }

                if (message.CrawlerError != null)
                {
                    await _errorOutput.SendAsync(message.CrawlerError);
                }
            }
        }

        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader,
            CrawlerMessage messageValue,
            ISourceBlock<CrawlerMessage> source,
            bool consumeToAccept)
        {
            return _input.OfferMessage(
                messageHeader, messageValue, source, consumeToAccept);
        }

        public void Complete()
        {
            _input.Complete();
        }

        public void Fault(Exception exception)
        {
            _input.Fault(exception);
        }

        public Task Completion { get; }
    }
}
