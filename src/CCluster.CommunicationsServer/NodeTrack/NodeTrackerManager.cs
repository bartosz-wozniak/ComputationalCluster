using System.Threading;
using CCluster.Common;
using log4net;

namespace CCluster.CommunicationsServer.NodeTrack
{
    public class NodeTrackerManager : SoftThread
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly INodeTracker nodeTracker;
        private readonly CommunicationsServerConfiguration csConfiguration;

        public NodeTrackerManager(INodeTracker nodeTracker, CommunicationsServerConfiguration csConfiguration)
        {
            this.nodeTracker = nodeTracker;
            this.csConfiguration = csConfiguration;
        }

        public override void Start()
        {
            logger.Info("Starting node tracker thread.");
            base.Start();
        }

        public override void Stop()
        {
            logger.Info("Stopping node tracker thread.");
            base.Stop();
        }

        protected override void ThreadMain(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                token.WaitHandle.WaitOne((int)csConfiguration.CommunicationsTimeoutTimeSpan.TotalMilliseconds / 4);
                if (token.IsCancellationRequested)
                {
                    break;
                }
                nodeTracker.DiscardOutdatedNodes();
            }
        }
    }
}
