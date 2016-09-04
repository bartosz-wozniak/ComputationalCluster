using System;
using System.Net;

namespace CCluster.Common.Communication
{
    public interface ITcpClient : IDisposable
    {
        IPEndPoint RemoteEndpoint { get; }

        bool Connected { get; }
        void Connect(IPAddress address, int port);
        INetworkStream GetStream();
    }
}
