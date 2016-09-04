using CCluster.Common.Configuration;
using CCluster.Common.Configuration.Reader;
using CCluster.Common.Tests.Configuration.TestObjects;
using FakeItEasy;
using Shouldly;

namespace CCluster.Common.Tests.Configuration.Tests
{
    public class ConfigurationBothSourcesTests
    {
        public void Valid_configuration_for_node()
        {
            string json = @"{""CsAddress"": ""192.168.0.1"",""CsPort"": 5050}";
            string[] args = new[] { "--address", "192.168.0.2", "--port", "5050" };
            
            IContentReader contentReader = A.Fake<IContentReader>();
            A.CallTo(() => contentReader.Read("test")).Returns(json);
            IConfigurationProvider confProvider = new ConfigurationProvider(contentReader);

            var result = confProvider.LoadConfiguration<TestNodeConfiguration>("test", args);
            result.CsAddress.ShouldBe("192.168.0.2");
            result.CsPort.ShouldBe(5050);
        }

        public void Valid_configuration_for_server_without_bakcup()
        {
            string json = @"{""ListeningPortNumber"": 5050,""CommunicationsTimeout"": 50}";
            string[] args = new[] { "--port", "666", "--t", "50", };

            IContentReader contentReader = A.Fake<IContentReader>();
            A.CallTo(() => contentReader.Read("test")).Returns(json);
            IConfigurationProvider confProvider = new ConfigurationProvider(contentReader);

            var result = confProvider.LoadConfiguration<TestServerConfiguration>("test", args);
            result.ListeningPortNumber.ShouldBe(666);
            result.IsBackup.ShouldBe(false);
            result.CommunicationsTimeout.ShouldBe(50);
        }

        public void Valid_configuration_for_server_with_bakcup()
        {
            string json = @"{
""ListeningPortNumber"": 5050,
""CommunicationsTimeout"": 50,
""IsBackup"": true,
""MasterServerListeningPortNumber"": 6060,
""MasterServerAddress"": ""192.456""}";
            string[] args = new[] { "--port", "666", "--t", "50", "--backup", "--maddress", "192.456" };

            IContentReader contentReader = A.Fake<IContentReader>();
            A.CallTo(() => contentReader.Read("test")).Returns(json);
            IConfigurationProvider confProvider = new ConfigurationProvider(contentReader);

            var result = confProvider.LoadConfiguration<TestServerConfiguration>("test",args);
            result.ListeningPortNumber.ShouldBe(666);
            result.IsBackup.ShouldBe(true);
            result.CommunicationsTimeout.ShouldBe(50);
            result.MasterServerAddress.ShouldBe("192.456");
            result.MasterServerListeningPortNumber.ShouldBe(6060);
        }
    }
}
