using System;
using System.Net;
using CCluster.Common.Configuration.Reader;

namespace CCluster.Common.Configuration
{
    public sealed class Loader
    {
        private readonly IConfigurationProvider configProvider;
        private readonly ICsConfigurationProvider csConfigProvider;

        public Loader(IConfigurationProvider configProvider, ICsConfigurationProvider csConfigProvider)
        {
            this.configProvider = configProvider;
            this.csConfigProvider = csConfigProvider;
        }

        public TConfig Load<TConfig>(Func<TConfig, string> addressGetter, Func<TConfig, int> portGetter,
            string filename = Constants.ConfigFileName)
            where TConfig : class, new()
        {
            var cfg = configProvider.LoadConfiguration<TConfig>(filename, Environment.GetCommandLineArgs());
            var address = IPAddress.Parse(addressGetter(cfg));
            var port = portGetter(cfg);
            csConfigProvider.ChangeConfiguration(address, port);
            return cfg;
        }
    }
}
