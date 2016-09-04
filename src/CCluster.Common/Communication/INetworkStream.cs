using System;

namespace CCluster.Common.Communication
{
    public interface INetworkStream : IDisposable
    {
        bool Connected { get; }
        bool DataAvailable { get; }
        int Timeout { get; set; }

        void Write(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
    }
}
