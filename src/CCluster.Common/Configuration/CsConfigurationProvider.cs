using System.Net;

namespace CCluster.Common.Configuration
{
    public class CsConfigurationProvider : ICsConfigurationProvider
    {
        private readonly Config config = new Config();

        public ICsConfiguration Configuration
        {
            get
            {
                return config;
            }
        }

        public void ChangeConfiguration(IPAddress address, int port)
        {
            config.Address = address;
            config.Port = port;
        }

        private sealed class Config : ICsConfiguration
        {
            public IPAddress Address { get; set; }
            public int Port { get; set; }
        }
    }
}
