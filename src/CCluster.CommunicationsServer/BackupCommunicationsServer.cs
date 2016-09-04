using System;
using System.Net;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Status;
using CCluster.CommunicationsServer.Messaging;
using CCluster.CommunicationsServer.Notifications;
using CCluster.CommunicationsServer.Storage;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer
{
    public class BackupCommunicationsServer : IMainServer, INotificationHandler<SwitchedToPrimary>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly BackupInputMessageListener msgListener;
        private readonly StatusManager statusManager;
        private readonly Func<IPEndPoint, IServerClient> serverClientFactory;
        private readonly CommunicationsServerConfiguration cfg;
        private readonly CommunicationsServer primaryServer;
        private readonly CommunicationServerStorage communicationServerStorage;

        private bool worksAsBackup = true;

        private readonly object lockObj = new object();

        public BackupCommunicationsServer(BackupInputMessageListener msgListener, StatusManager statusManager,
            CommunicationsServerConfiguration cfg, CommunicationsServer primaryServer, Func<IPEndPoint, IServerClient> serverClientFactory, 
            CommunicationServerStorage communicationServerStorage)
        {
            this.msgListener = msgListener;
            this.statusManager = statusManager;
            this.serverClientFactory = serverClientFactory;
            this.cfg = cfg;
            this.primaryServer = primaryServer;
            this.communicationServerStorage = communicationServerStorage;
        }

        public void Start()
        {
            communicationServerStorage.IsBackup = true;
            statusManager.Start();
            TryRegister();
            msgListener.Start();
        }

        public void Stop()
        {
            if (worksAsBackup)
            {
                StopBackup();
            }
            else
            {
                primaryServer.Stop();
            }
        }

        private void TryRegister()
        {
            var client = serverClientFactory(new IPEndPoint(IPAddress.Any, cfg.Port));
            try
            {
                client.Send(new RegisterMessage { Type = Constants.NodeTypes.CommunicationsServer });
            }
            catch (Exception ex)
            {
                logger.Fatal("Cannot register backup server in primary CS, aborting.", ex);
                throw;
            }
        }

        private void StopBackup()
        {
            msgListener.Stop();
            statusManager.Stop();
            communicationServerStorage.IsBackup = false;
        }

        public void Handle(SwitchedToPrimary notification)
        {
            worksAsBackup = false;
            StopBackup();
            primaryServer.Start();
        }
    }
}
