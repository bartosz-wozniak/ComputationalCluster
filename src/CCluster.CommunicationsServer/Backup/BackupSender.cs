using System;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Notifications;
using CCluster.Messages;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Backup
{
    public class BackupSender : IBackupSender
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IMessagesSender sender;
        private readonly IBackupServerManager backupServers;
        private readonly Func<ITcpClient> clientFactory;
        private readonly IMediator mediator;

        public BackupSender(IMessagesSender sender, IBackupServerManager backupServers, Func<ITcpClient> clientFactory,
            IMediator mediator)
        {
            this.sender = sender;
            this.backupServers = backupServers;
            this.clientFactory = clientFactory;
            this.mediator = mediator;
        }

        public void Send(IMessage message)
        {
            var followingNodes = backupServers.GetFollowingNodes();
            if (followingNodes.Count == 0)
            {
                logger.Debug($"There is no backup servers. Discarding message: {message}");
            }
            else
            {
                foreach (var node in followingNodes)
                {
                    logger.Debug($"Trying to send to {node.Address}:{node.Port}.");
                    try
                    {
                        using (var client = clientFactory())
                        {
                            client.Connect(node.Address, node.Port);
                            sender.Send(message, client.GetStream());
                        }

                        // We only want to send a message to the next alive backup server
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Cannot send message to {node.Address}:{node.Port}, assuming the backup CS is dead.", ex);
                        mediator.Publish(new NodeDead(node.Id));
                    }
                }
            }
        }
    }
}
