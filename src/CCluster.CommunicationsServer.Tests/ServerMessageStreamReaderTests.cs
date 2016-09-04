using CCluster.Common.Communication.Messaging;
using CCluster.CommunicationsServer.Exceptions;
using CCluster.CommunicationsServer.Messaging;
using CCluster.Messages;
using CCluster.TestsHelper;
using FakeItEasy;
using Shouldly;

namespace CCluster.CommunicationsServer.Tests
{
    public class ServerMessageStreamReaderTests
    {
        private readonly IMessageDeserializer deserializer;
        private readonly NetworkMemoryStream stream;

        private readonly ServerMessageStreamReader streamReader;
        private readonly IMessage preparedMessage;

        public ServerMessageStreamReaderTests()
        {
            deserializer = A.Fake<IMessageDeserializer>();
            stream = A.Fake<NetworkMemoryStream>(o => o.CallsBaseMethods());

            preparedMessage = A.Fake<IMessage>();

            A.CallTo(() => deserializer.Deserialize(null, 0, 0))
                .WithAnyArguments()
                .Returns(preparedMessage);

            streamReader = new ServerMessageStreamReader(stream, deserializer);
        }

        public void ShouldReadAndSerializeDataFromStream()
        {
            PutData(1, 2);
            streamReader.ReadAvailable();
            A.CallTo(() => stream.Read(null, 0, 0)).WithAnyArguments().MustHaveHappened();
            A.CallTo(() => deserializer.Deserialize(null, 0, 0)).WithAnyArguments().MustHaveHappened();
        }

        public void ShouldThrowExceptionWhenStreamIsEmpty()
        {
            Should.Throw<CannotReadMessageException>(() => streamReader.ReadAvailable());
        }

        private void PutData(params byte[] data)
        {
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
        }
    }
}