using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CCluster.Client
{
    /// <summary>
    /// Simple <see cref="IMediator"/> implementation that caches the messages. Temporary.
    /// </summary>
    public class SimpleMediator : IMediator
    {
        private readonly BlockingCollection<object> messages = new BlockingCollection<object>(new ConcurrentQueue<object>());

        public TMessage GetLast<TMessage>()
        {
            var msg = messages.Take();
            if (msg is TMessage)
            {
                return (TMessage)msg;
            }
            throw new InvalidOperationException("Last message is of different type!");
        }

        public void Publish(INotification notification)
        {
            messages.Add(notification);
        }

        public TResponse Send<TResponse>(IRequest<TResponse> request)
        {
            messages.Add(request);
            return default(TResponse);
        }

        public Task<TResponse> SendAsync<TResponse>(IAsyncRequest<TResponse> request)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendAsync<TResponse>(ICancellableAsyncRequest<TResponse> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(IAsyncNotification notification)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(ICancellableAsyncNotification notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
