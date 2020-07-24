using System;
using System.Threading.Tasks.Dataflow;

namespace OHunt.Web.Dataflow
{
    /// <summary>
    /// To create DataCachingBlock.
    ///
    /// The block caches its input and output 
    /// </summary>
    public class DataCachingBlockFactory
    {
        public static IPropagatorBlock<DataCachingMessage<T>, T> CreateBlock<T>(
            int capacity) where T : class
        {
            var cache = new T[capacity];
            var end = 0;

            var source = new BufferBlock<T>();
            var target = new ActionBlock<DataCachingMessage<T>>(async item =>
            {
                if (item.Discard)
                {
                    end = 0;
                }

                if (item.Entity != null)
                {
                    if (end >= capacity)
                    {
                        throw new InvalidOperationException("The cache is full");
                    }

                    cache[end++] = item.Entity;
                }

                if (item.Submit)
                {
                    for (var i = 0; i < end; i++)
                    {
                        await source.SendAsync(cache[i]);
                    }

                    end = 0;
                }
            });

            target.Completion.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    ((IDataflowBlock) source).Fault(task.Exception);
                }
                else
                {
                    // TODO: handle cancel
                    source.Complete();
                }
            });

            return DataflowBlock.Encapsulate(target, source);
        }
    }

    /// <summary>
    /// The message sent to DataCachingBlock
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct DataCachingMessage<T>
        where T : class
    {
        private DataCachingMessage(T? entity, bool submit, bool discard)
        {
            Entity = entity;
            Submit = submit;
            Discard = discard;
        }

        public static DataCachingMessage<T> OfEntity(T entity, bool submit = false)
        {
            return new DataCachingMessage<T>(entity, submit, false);
        }

        public static DataCachingMessage<T> SubmitMessage
            => new DataCachingMessage<T>(null, true, false);

        public static DataCachingMessage<T> DiscardMessage
            => new DataCachingMessage<T>(null, false, true);

        /// <summary>
        /// The entity to cache
        /// </summary>
        public T? Entity { get; }

        /// <summary>
        /// Submit all cached entities
        /// </summary>
        public bool Submit { get; }

        /// <summary>
        /// Discard all cached entities
        ///
        /// If it is true, Submit and Entity fields are ignored.
        /// </summary>
        public bool Discard { get; }
    }
}
