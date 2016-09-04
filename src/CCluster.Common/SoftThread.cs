using System;
using System.Threading;

namespace CCluster.Common
{
    public abstract class SoftThread : IDisposable
    {
        private CancellationTokenSource cts;
        private Thread thread;

        public bool WasStarted { get; private set; }

        public object SyncRoot { get; } = new object();

        public SoftThread()
        {
        }

        public virtual void Start()
        {
            lock (SyncRoot)
            {
                if (WasStarted)
                {
                    return;
                }

                cts = new CancellationTokenSource();
                thread = new Thread(ThreadMainInternal);
                thread.Start();

                WasStarted = true;
            }
        }

        public virtual void Stop()
        {
            lock (SyncRoot)
            {
                if (!WasStarted)
                {
                    return;
                }

                cts.Cancel();
                PreStopEvent();
                thread.Join();

                cts.Dispose();

                thread = null;
                cts = null;

                WasStarted = false;
            }
        }

        protected abstract void ThreadMain(CancellationToken token);

        protected virtual void PreStopEvent()
        { }

        private void ThreadMainInternal()
        {
            ThreadMain(cts.Token);
        }

        void IDisposable.Dispose()
        {
            Stop();
        }
    }
}
