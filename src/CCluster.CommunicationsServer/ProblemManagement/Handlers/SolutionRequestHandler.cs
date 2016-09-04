using System.Linq;
using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Messaging;
using CCluster.Messages;
using MediatR;

namespace CCluster.CommunicationsServer.ProblemManagement.Handlers
{
    public class SolutionRequestHandler : RequestHandler<ClientMessage<SolutionRequest>>
    {
        private readonly IProblemManager manager;
        private readonly IMessagesSender sender;

        public SolutionRequestHandler(IProblemManager manager, IMessagesSender sender)
        {
            this.manager = manager;
            this.sender = sender;
        }

        protected override void HandleCore(ClientMessage<SolutionRequest> message)
        {
            var problem = manager.GetProblemForSending(message.Message.Id);
            if (problem == null)
            {
                message.Respond(sender, new Solutions());
            }
            else
            {
                var response = new Solutions
                {
                    Id = message.Message.Id,
                    ProblemType = problem.Type,
                    SolutionsList = problem.SubProblems.Select(s => new Solution
                    {
                        TaskId = s.Id,
                        Type = problem.State == ProblemState.Completed ? SolutionType.Final : s.IsFinished ? SolutionType.Partial : SolutionType.Ongoing,
                        Data = s.Result
                    }).ToArray()
                };
                message.Respond(sender, response);
                manager.MarkAsSent(problem.Id);
            }
        }
    }
}
