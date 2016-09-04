using MediatR;

namespace CCluster.Messages.Notifications
{
    /// <summary>
    /// One of the threads has started working on a problem.
    /// </summary>
    public sealed class WorkTaken : INotification
    {
        public int ThreadId { get; }
        public ulong ProblemInstanceId { get; }
        public ulong TaskId { get; }
        public string ProblemType { get; }

        public WorkTaken(int threadId, ulong problemId, ulong taskId, string type)
        {
            this.ThreadId = threadId;
            this.ProblemInstanceId = problemId;
            this.TaskId = taskId;
            this.ProblemType = type;
        }
    }
}
