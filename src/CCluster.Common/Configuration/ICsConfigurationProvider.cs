using System.Net;

namespace CCluster.Common.Configuration
{
    public interface ICsConfigurationProvider
    {
        ICsConfiguration Configuration { get; }
        void ChangeConfiguration(IPAddress address, int port);
    }
}
