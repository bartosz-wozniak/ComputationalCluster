using CCluster.Common.Configuration.Reader;
using CCluster.Common.Tests.Configuration.TestObjects;
using FakeItEasy;
using Shouldly;

namespace CCluster.Common.Tests.Configuration.Tests
{
    public class CmdConfigurationTests
    {
        private readonly IConfigurationProvider confProvider;

        public CmdConfigurationTests()
        {
            var contentProvider = A.Fake<IContentReader>();
            A.CallTo(() => contentProvider.Read(string.Empty)).WithAnyArguments().Returns(string.Empty);
            confProvider = new ConfigurationProvider(contentProvider);
        }

        public void Valid_cmd_configuration_for_node()
        {
            string[] args = new[] { "--address", "192.168.0.1", "--port", "5050" };
            var result = confProvider.LoadConfiguration<TestNodeConfiguration>(null, args);
            result.CsAddress.ShouldBe("192.168.0.1");
            result.CsPort.ShouldBe(5050);
        }

        public void Valid_cmd_configuration_for_server_without_bakcup()
        {
            string[] args = new[] { "--port", "5050", "--t", "50", };
            var result = confProvider.LoadConfiguration<TestServerConfiguration>(null, args);
            result.ListeningPortNumber.ShouldBe(5050);
            result.IsBackup.ShouldBe(false);
            result.CommunicationsTimeout.ShouldBe(50);
        }

        public void Valid_cmd_configuration_for_server_with_bakcup()
        {
            string[] args = new[] { "--port", "5050", "--t", "50", "--backup", "--mport", "6060", "--maddress", "192.456" };
            var result = confProvider.LoadConfiguration<TestServerConfiguration>(null, args);
            result.ListeningPortNumber.ShouldBe(5050);
            result.IsBackup.ShouldBe(true);
            result.CommunicationsTimeout.ShouldBe(50);
            result.MasterServerAddress.ShouldBe("192.456");
            result.MasterServerListeningPortNumber.ShouldBe(6060);
        }
    }
}
