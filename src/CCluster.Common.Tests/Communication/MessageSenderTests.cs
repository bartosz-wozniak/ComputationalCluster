using System;
using System.Collections.Generic;
using System.Linq;
using CCluster.Common.Communication.Messaging;
using CCluster.Messages;
using CCluster.TestsHelper;
using FakeItEasy;
using Shouldly;

namespace CCluster.Common.Tests.Communication
{
    public class MessageSenderTests : IDisposable
    {
        private readonly NetworkMemoryStream stream;
        private readonly IMessageSerializer serializer;

        private readonly IMessagesSender sender;

        public MessageSenderTests()
        {
            stream = new NetworkMemoryStream();
            serializer = A.Fake<IMessageSerializer>();
            A.CallTo(() => serializer.Serialize(null)).WithAnyArguments().Returns(new byte[0]);

            sender = new MessagesSender(serializer);
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        public void Calls_the_serializer_to_serialize_single_message()
        {
            Execute();

            A.CallTo(() => serializer.Serialize(null)).WithAnyArguments().MustHaveHappened();
        }

        public void Passes_the_message_to_the_serializer()
        {
            var msg = A.Fake<IMessage>();

            Execute(msg);

            A.CallTo(() => serializer.Serialize(msg)).MustHaveHappened();
        }

        public void Calls_the_serializer_with_collection_of_messages()
        {
            var msgs = A.CollectionOfFake<IMessage>(3);

            Execute(msgs.ToArray());

            foreach (var msg in msgs)
            {
                A.CallTo(() => serializer.Serialize(msg)).MustHaveHappened();
            }
        }

        public void When_single_message_is_being_sent_It_copies_serialized_message_to_the_output()
        {
            var msgs = PrepareMessages(1);

            Execute(msgs.Item1);

            var result = stream.ToArray();
            result.ShouldBe(msgs.Item2[0]);
        }

        public void When_multiple_messages_are_passed_They_are_separated_with_correct_byte_in_the_output()
        {
            var msgs = PrepareMessages(2);

            Execute(msgs.Item1);

            var result = stream.ToArray().Where((_, i) => i % 2 == 1);
            result.ShouldAllBe(b => b == 23);
        }

        public void When_multiple_messages_are_passed_They_are_put_in_output_in_correct_order()
        {
            var msgs = PrepareMessages();

            Execute(msgs.Item1);

            var result = stream.ToArray().Where((_, i) => i % 2 == 0);
            result.ShouldBe(msgs.Item2.Select(d => d[0]));
        }

        public void When_multiple_messages_are_passed_Output_looks_as_expected()
        {
            var msgs = PrepareMessages();
            var correctResult = new byte[] { 5, 23, 6, 23, 7 };

            Execute(msgs.Item1);

            var result = stream.ToArray();
            result.ShouldBe(correctResult);
        }

        public void When_sending_single_message_The_message_gets_serialized()
        {
            var msg = A.Fake<IMessage>();

            sender.Send(msg, stream);

            A.CallTo(() => serializer.Serialize(msg)).MustHaveHappened();
        }

        public void When_sending_single_message_The_serialized_message_gets_written_as_is()
        {
            var msg = A.Fake<IMessage>();
            var correctResult = new byte[] { 9, 7, 8 };

            A.CallTo(() => serializer.Serialize(msg)).Returns(correctResult);

            sender.Send(msg, stream);

            var result = stream.ToArray();
            result.ShouldBe(correctResult);
        }

        private Tuple<IMessage[], byte[][]> PrepareMessages(int count = 3)
        {
            var msgs = A.CollectionOfFake<IMessage>(count).ToArray();
            var data = Enumerable.Range(5, count).Select(b => new[] { (byte)b }).ToArray();
            var result = msgs.Zip(data, (m, d) => Tuple.Create(m, d));
            SetSerializerData(result);
            return Tuple.Create(msgs, data);
        }

        private void SetSerializerData(IEnumerable<Tuple<IMessage, byte[]>> data)
        {
            foreach (var item in data)
            {
                A.CallTo(() => serializer.Serialize(item.Item1)).Returns(item.Item2);
            }
        }

        private void Execute(params IMessage[] msgs)
        {
            if (msgs == null || msgs.Length == 0)
            {
                msgs = new[] { A.Fake<IMessage>() };
            }
            sender.Send(msgs, stream);
        }
    }
}
