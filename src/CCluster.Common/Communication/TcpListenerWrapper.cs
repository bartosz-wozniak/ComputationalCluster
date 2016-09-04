using System.Net.Sockets;

namespace CCluster.Common.Communication
{
    public sealed class TcpListenerWrapper : ITcpListener
    {
        private readonly TcpListener listener;

        public TcpListenerWrapper(TcpListener listener)
        {
            this.listener = listener;
        }

        public ITcpClient AcceptTcpClient()
        {
            return new TcpClientWrapper(listener.AcceptTcpClient());
        }

        public void Start()
        {
            listener.Start();
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
