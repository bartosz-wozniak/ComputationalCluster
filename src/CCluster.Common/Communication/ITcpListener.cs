using System;

namespace CCluster.Common.Communication
{
    public interface ITcpListener : IDisposable
    {
        void Start();
        ITcpClient AcceptTcpClient();
    }
}
