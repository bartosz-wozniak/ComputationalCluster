namespace CCluster.Common.Configuration.Reader
{
    public interface IConfigurationProvider
    {
        T LoadConfiguration<T>(string fileName, string[] args = null) where T : class, new();
    }
}
