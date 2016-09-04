using System.IO;
using CCluster.Common.Communication;

namespace CCluster.TestsHelper
{
    public class NetworkMemoryStream : MemoryStream, INetworkStream
    {
        public bool Connected { get; private set; } = true;

        public bool DataAvailable => Position < Length;

        public int Timeout { get; set; }

        public override void Close()
        {
            Connected = false;
        }
    }
}
