using CommandLine;
using Newtonsoft.Json;

namespace CCluster.Common.Configuration.Reader
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IContentReader contentReader;

        public ConfigurationProvider(IContentReader contentReader)
        {
            this.contentReader = contentReader;
        }

        public T LoadConfiguration<T>(string fileName, string[] args = null) where T : class, new()
        {
            var config = LoadConfigurationFromFile<T>(fileName) ?? new T();
            LoadConfigurationFromCmd<T>(config, args);
            return config;
        }

        private T LoadConfigurationFromFile<T>(string fileName)
            where T : class
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            string fileContent = contentReader.Read(fileName);
            return JsonConvert.DeserializeObject<T>(fileContent);
        }

        private void LoadConfigurationFromCmd<T>(T obj, string[] args)
            where T : new()
        {
            if (args != null)
            {
                Parser.Default.ParseArguments(() => obj, args);
            }
        }
    }
}
