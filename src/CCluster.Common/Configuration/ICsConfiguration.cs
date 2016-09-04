using System.Net;

namespace CCluster.Common.Configuration
{
    public interface ICsConfiguration
    {
        IPAddress Address { get; }
        int Port { get; }
    }
}
