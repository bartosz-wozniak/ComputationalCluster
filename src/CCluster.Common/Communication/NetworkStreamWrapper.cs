using System.Net.Sockets;

namespace CCluster.Common.Communication
{
    public class NetworkStreamWrapper : INetworkStream
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;

        public bool Connected
        {
            get { return client.IsConnected(); }
        }

        public bool DataAvailable
        {
            get { return stream.DataAvailable; }
        }

        public int Timeout
        {
            get { return stream.ReadTimeout; }
            set { stream.ReadTimeout = stream.WriteTimeout = value; }
        }

        public NetworkStreamWrapper(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        public override string ToString()
        {
            return client.Client.RemoteEndPoint.ToString();
        }
    }
}
