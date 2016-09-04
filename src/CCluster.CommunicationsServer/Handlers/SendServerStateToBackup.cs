using System;
using System.Collections.Generic;
using System.Linq;
using CCluster.Common;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.NodeTrack;
using CCluster.CommunicationsServer.Notifications;
using CCluster.CommunicationsServer.ProblemManagement;
using CCluster.CommunicationsServer.Storage;
using CCluster.Messages;
using CCluster.Messages.Register;
using log4net;
using MediatR;

namespace CCluster.CommunicationsServer.Handlers
{
    public class SendServerStateToBackup : INotificationHandler<NodeRegistered>
    {
        private readonly ILog logger = LogProvider.GetCurrentClassLogger();

        private readonly IMessagesSender sender;
        private readonly Func<ITcpClient> clientFactory;
        private readonly ICsDataStore dataStore;
        private readonly IMediator mediator;
        private readonly IProblemManager problemManager;
        private readonly CommunicationServerStorage communicationServerStorage;

        public SendServerStateToBackup(IMessagesSender sender, Func<ITcpClient> clientFactory, ICsDataStore dataStore,
            IMediator mediator, IProblemManager problemManager, CommunicationServerStorage communicationServerStorage)
        {
            this.sender = sender;
            this.clientFactory = clientFactory;
            this.dataStore = dataStore;
            this.mediator = mediator;
            this.problemManager = problemManager;
            this.communicationServerStorage = communicationServerStorage;
        }

        public void Handle(NodeRegistered notification)
        {
            //check if current node is primary CS
            if (notification.Message.Type == Constants.NodeTypes.CommunicationsServer && !communicationServerStorage.IsBackup)
            {
                logger.Info("New backup server has been registered, sending him all availble info.");

                using (var client = clientFactory())
                {
                    try
                    {
                        client.Connect(notification.MessageSource.Address, notification.MessageSource.Port);

                        IEnumerable<IMessage> regMsgs = GetRegisterMessages();
                        IEnumerable<IMessage> problemMsgs = GetProblemMessages();

                        sender.Send(regMsgs.Concat(problemMsgs).ToList(), client.GetStream());
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Newly registered node {notification.Message.Id} stopped responding, marking as dead", ex);
                        mediator.Publish(new NodeDead(notification.Message.Id));
                    }
                }
            }
        }

        private IEnumerable<RegisterMessage> GetRegisterMessages()
        {
            var nodes = dataStore.ConnectedNodes;
            return nodes.Select(n => new RegisterMessage
            {
                Id = n.Id,
                Type = n.Type,
                SolvableProblems = n.SupportedProblems.ToArray()
            }).ToList();
        }

        private IEnumerable<ProblemSync> GetProblemMessages()
        {
            var problems = problemManager.GetProblemsForSync();
            foreach (var p in problems)
            {
                yield return new ProblemSync
                {
                    Id = p.Id,
                    Data = p.Data,
                    State = p.State,
                    Type = p.Type,
                    AssignedNode = p.AssignedNode,
                    SubProblems = p.SubProblems.Select(s => new SubProblemSync
                    {
                        Id = s.Id,
                        AssignedNode = s.AssignedNode,
                        Data = s.Data,
                        IsFinished = s.IsFinished,
                        Result = s.Result
                    }).ToArray()
                };
            }
        }
    }
}
