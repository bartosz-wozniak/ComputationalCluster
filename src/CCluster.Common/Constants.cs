namespace CCluster.Common
{
    public static class Constants
    {
        public const byte MessageSeparator = 23;
        public const string ConfigFileName = "config.json";

        public const string ProblemName = "DvrpTaskSolver";
        public const int BufferSize = 1024*1024*100;
        public const int NumberOfProblemsSendByOnce = 250;


        public static class NodeTypes
        {
            public const string CommunicationsServer = "CommunicationsServer";
            public const string ComputationalNode = "ComputationalNode";
            public const string TaskManager = "TaskManager";
        }
    }
}
