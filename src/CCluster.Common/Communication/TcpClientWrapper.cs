using System;
using System.Net;
using System.Net.Sockets;

namespace CCluster.Common.Communication
{
    public sealed class TcpClientWrapper : ITcpClient
    {
        const int BufferSize = 1024 * 1024 * 100; // 100MB
        private readonly TcpClient client;
        private readonly Lazy<NetworkStreamWrapper> stream;

        public bool Connected
        {
            get { return client.IsConnected(); }
        }

        public IPEndPoint RemoteEndpoint
        {
            get { return (IPEndPoint)client.Client.RemoteEndPoint; }
        }

        public TcpClientWrapper()
        {
            client = new TcpClient()
            {
                SendBufferSize = BufferSize,
                ReceiveBufferSize = BufferSize
            };
            stream = new Lazy<NetworkStreamWrapper>(() => new NetworkStreamWrapper(this.client));
        }

        public TcpClientWrapper(IPEndPoint endPoint)
        {
            client = new TcpClient(endPoint)
            {
                SendBufferSize = BufferSize,
                ReceiveBufferSize = BufferSize
            };
            stream = new Lazy<NetworkStreamWrapper>(() => new NetworkStreamWrapper(this.client));
        }

        internal TcpClientWrapper(TcpClient client)
        {
            this.client = client;
            stream = new Lazy<NetworkStreamWrapper>(() => new NetworkStreamWrapper(this.client));
        }

        public void Connect(IPAddress address, int port)
        {
            client.Connect(address, port);
        }

        public INetworkStream GetStream()
        {
            return stream.Value;
        }

        public void Dispose()
        {
            client.Close();
            client.Dispose();
        }

        public override string ToString()
        {
            return client.Client.RemoteEndPoint.ToString();
        }
    }
}
