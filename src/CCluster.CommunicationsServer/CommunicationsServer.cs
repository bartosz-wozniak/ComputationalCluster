using CCluster.Common;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.NodeTrack;
using CCluster.CommunicationsServer.Storage;
using log4net;

namespace CCluster.CommunicationsServer
{
    public class CommunicationsServer : IMainServer
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly PrimaryInputMessageListener msgListener;
        private readonly NodeTrackerManager nodeTrackerManager;

        public CommunicationsServer(PrimaryInputMessageListener msgListener, NodeTrackerManager nodeTracker)
        {
            this.msgListener = msgListener;
            this.nodeTrackerManager = nodeTracker;
        }

        public void Start()
        {
            logger.Info("Starting primary CS server.");
            nodeTrackerManager.Start();
            msgListener.Start();
            logger.Info("Primary CS server started.");
        }

        public void Stop()
        {
            logger.Info("Stopping primary CS server.");
            msgListener.Stop();
            nodeTrackerManager.Stop();
            logger.Info("Primary CS server stopped.");
        }
    }
}