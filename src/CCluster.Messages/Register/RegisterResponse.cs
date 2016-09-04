namespace CCluster.Messages.Register
{
    public class RegisterResponse : IMessage
    {
        public ulong Id { get; set; }
        public uint Timeout { get; set; }
    }
}