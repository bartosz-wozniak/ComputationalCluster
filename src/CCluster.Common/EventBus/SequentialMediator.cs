using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MediatR;

namespace CCluster.Common.EventBus
{
    public sealed class SequentialMediator : SoftThread, IMediator
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly BlockingCollection<QueueItem> queuedMessages
            = new BlockingCollection<QueueItem>(new ConcurrentQueue<QueueItem>());

        private readonly Mediator innerMediator;

        public SequentialMediator(SingleInstanceFactory single, MultiInstanceFactory multi)
        {
            innerMediator = new Mediator(single, multi);
        }

        public void Publish(INotification notification)
        {
            queuedMessages.Add(new QueueItem(notification));
        }

        public TResponse Send<TResponse>(IRequest<TResponse> request)
        {
            if (typeof(TResponse) != typeof(Unit))
            {
                throw new NotSupportedException("Only Unit requests are supported.");
            }

            queuedMessages.Add(new QueueItem((IRequest<Unit>)request));
            return default(TResponse);
        }

        public Task PublishAsync(IAsyncNotification notification)
        {
            throw new NotSupportedException();
        }

        public Task PublishAsync(ICancellableAsyncNotification notification, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<TResponse> SendAsync<TResponse>(IAsyncRequest<TResponse> request)
        {
            throw new NotSupportedException();
        }

        public Task<TResponse> SendAsync<TResponse>(ICancellableAsyncRequest<TResponse> request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        protected override void ThreadMain(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                QueueItem request;
                try
                {
                    request = queuedMessages.Take(token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                //try
                //{
                    if (request.IsNotification)
                    {
                        innerMediator.Publish((INotification)request.Request);
                    }
                    else
                    {
                        innerMediator.Send((IRequest<Unit>)request.Request);
                    }
                //}
                //catch (Exception ex)
                //{
                //    logger.Fatal($"Handler for {request} failed, re-throwing.", ex);
                //    throw;
                //}
            }
        }

        private struct QueueItem
        {
            public object Request;
            public bool IsNotification;

            public QueueItem(INotification notification)
            {
                Request = notification;
                IsNotification = true;
            }

            public QueueItem(IRequest<Unit> request)
            {
                Request = request;
                IsNotification = false;
            }
        }
    }
}
