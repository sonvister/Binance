using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Binance.Utility
{
    public sealed class QueuedProcessor<T>
    {
        #region Private Fields

        //private BufferBlock<T> _bufferBlock;
        private ActionBlock<T> _actionBlock;

        #endregion Private Fields

        #region Constructors

        public QueuedProcessor(Action<T> action)
        {
            Throw.IfNull(action, nameof(action));

            //_bufferBlock = new BufferBlock<T>(new DataflowBlockOptions
            //{
            //    EnsureOrdered = true,
            //    CancellationToken = CancellationToken.None,
            //    BoundedCapacity = DataflowBlockOptions.Unbounded,
            //    MaxMessagesPerTask = DataflowBlockOptions.Unbounded
            //});

            _actionBlock = new ActionBlock<T>(action,
                new ExecutionDataflowBlockOptions
                {
                    //BoundedCapacity = 1,
                    BoundedCapacity = DataflowBlockOptions.Unbounded,
                    EnsureOrdered = true,
                    //MaxMessagesPerTask = 1,
                    MaxDegreeOfParallelism = 1,
                    CancellationToken = CancellationToken.None,
                    SingleProducerConstrained = true
                });

            //_bufferBlock.LinkTo(_actionBlock);
        }

        public QueuedProcessor(Func<T, CancellationToken, Task> actionAsync, CancellationToken token = default)
        {
            Throw.IfNull(actionAsync, nameof(actionAsync));

            //_bufferBlock = new BufferBlock<T>(new DataflowBlockOptions
            //{
            //    EnsureOrdered = true,
            //    CancellationToken = token,
            //    BoundedCapacity = DataflowBlockOptions.Unbounded,
            //    MaxMessagesPerTask = DataflowBlockOptions.Unbounded
            //});

            _actionBlock = new ActionBlock<T>(t => actionAsync(t, token),
                new ExecutionDataflowBlockOptions
                {
                    //BoundedCapacity = 1,
                    BoundedCapacity = DataflowBlockOptions.Unbounded,
                    EnsureOrdered = true,
                    //MaxMessagesPerTask = 1,
                    MaxDegreeOfParallelism = 1,
                    CancellationToken = token,
                    SingleProducerConstrained = true
                });

            //_bufferBlock.LinkTo(_actionBlock);
        }

        #endregion Constructors

        #region Protected Methods

        public void Complete()
        {
            //_bufferBlock?.Complete();
            _actionBlock?.Complete();

            //_bufferBlock = null;
            _actionBlock = null;
        }

        #endregion Protected Methods

        #region Private Methods

        public void Post(T item)
        {
            // Provides buffering and single-threaded execution.
            //_bufferBlock.Post(item);
            _actionBlock.Post(item);
        }

        #endregion Private Methods
    }
}
