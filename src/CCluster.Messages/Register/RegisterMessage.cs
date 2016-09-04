namespace CCluster.Messages.Register
{
    public class RegisterMessage : IMessage
    {
        public ulong Id { get; set; }
        public string Type { get; set; }
        public bool Deregister { get; set; } = false;
        public string[] SolvableProblems { get; set; }
    }
}