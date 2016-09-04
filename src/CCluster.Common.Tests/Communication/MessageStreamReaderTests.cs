using System.IO;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Messaging;
using CCluster.Messages;
using CCluster.TestsHelper;
using FakeItEasy;
using FakeItEasy.Configuration;
using Shouldly;

namespace CCluster.Common.Tests.Communication
{
    public class MessageStreamReaderTests
    {
        private const byte Separator = Constants.MessageSeparator;
        private const byte FirstMsgData = 1;
        private const byte SecondMsgData = 2;

        private readonly NetworkMemoryStream stream;
        private readonly IMessageDeserializer deserializer;
        private readonly IMessage preparedMessage;

        private readonly IMessageStreamReader parser;

        public MessageStreamReaderTests()
        {
            stream = A.Fake<NetworkMemoryStream>(o => o.CallsBaseMethods());

            deserializer = A.Fake<IMessageDeserializer>();
            preparedMessage = A.Fake<IMessage>();

            A.CallTo(() => deserializer.Deserialize(null, 0, 0))
                .WhenArgumentsMatch(FirstMessage)
                .Returns(preparedMessage);

            parser = new MessageStreamReader(deserializer, stream);
        }

        public void Reads_data_from_the_stream()
        {
            PutData(1);

            parser.ReadAvailable();

            A.CallTo(() => stream.Read(null, 0, 0)).WithAnyArguments().MustHaveHappened();
        }

        public void When_there_is_no_separator_and_the_stream_remains_open_It_does_not_try_to_parse_a_message()
        {
            PutData(1, 2, 3);

            var result = parser.ReadAvailable();

            result.ShouldBeEmpty();
        }

        public void When_stream_gets_closed_Deserializes_the_message()
        {
            PutDataAndClose(FirstMsgData);

            var result = parser.ReadAvailable();

            result.Count.ShouldBe(1);
            result.ShouldContain(preparedMessage);
        }

        public void When_the_separator_is_present_Deserializes_the_message()
        {
            PutData(FirstMsgData, Separator, 2);

            var result = parser.ReadAvailable();

            result.Count.ShouldBe(1);
            result.ShouldContain(preparedMessage);
        }

        public void When_there_is_data_after_the_separator_It_does_not_try_to_deserialize_next_message()
        {
            PutData(2, Separator, FirstMsgData);

            var result = parser.ReadAvailable();

            result.Count.ShouldBe(1);
            result.ShouldNotContain(preparedMessage);
        }

        public void When_there_is_data_after_the_separator_It_caches_the_data_and_deserializes_next_message_in_case_of_stream_closing()
        {
            PutData(2, Separator, FirstMsgData);

            parser.ReadAvailable();
            stream.Close();

            var result = parser.ReadAvailable();
            result.Count.ShouldBe(1);
            result.ShouldContain(preparedMessage);
        }

        public void When_there_is_data_after_the_separator_It_caches_the_data_and_deserializes_next_message_in_case_of_new_separator_presence()
        {
            PutData(2, Separator, FirstMsgData);

            parser.ReadAvailable();
            PutDataAtTheEnd(Separator);

            var result = parser.ReadAvailable();
            result.Count.ShouldBe(1);
            result.ShouldContain(preparedMessage);
        }

        public void When_the_stream_is_closed_and_all_messages_have_been_parsed_Reports_the_stream_has_no_more_messages()
        {
            PutDataAndClose(1);

            parser.ReadAvailable();

            parser.MayHaveMessages.ShouldBeFalse();
        }

        public void When_the_stream_is_closed_and_there_is_still_some_data_Reports_the_stream_may_have_messages()
        {
            PutData(1);

            parser.ReadAvailable();
            stream.Close();

            parser.MayHaveMessages.ShouldBeTrue();
        }

        public void When_the_stream_is_still_open_Reports_it_may_have_messages()
        {
            PutData(1, Separator);

            parser.ReadAvailable();

            parser.MayHaveMessages.ShouldBeTrue();
        }

        private void PutData(params byte[] data)
        {
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
        }

        private void PutDataAndClose(params byte[] data)
        {
            PutData(data);
            stream.Close();
        }

        private void PutDataAtTheEnd(params byte[] data)
        {
            var orgPos = stream.Position;
            stream.Seek(0, SeekOrigin.End);
            stream.Write(data, 0, data.Length);
            stream.Position = orgPos;
        }

        private static bool FirstMessage(ArgumentCollection args)
        {
            var data = args.Get<byte[]>(0);
            var offset = args.Get<int>(1);
            var count = args.Get<int>(2);

            return count == 1 && data[offset] == FirstMsgData;
        }
    }
}
