#pragma warning disable CS0618 // Type or member is obsolete
using System;
using System.Linq;
using CCluster.Common.Communication.Exceptions;
using CCluster.Common.Communication.Messaging;
using CCluster.Messages;
using Shouldly;

namespace CCluster.Common.Tests.Communication
{
    public class SimpleMessageSerializerTests
    {
        private readonly SimpleMessageSerializer serializer = new SimpleMessageSerializer();

        private readonly TestMessage Msg = new TestMessage { Test = "test" };
        private readonly TestMessage2 Msg2 = new TestMessage2 { Test = 5 };

        public void Serializes_test_message()
        {
            var result = Should.NotThrow(() => serializer.Serialize(Msg));

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
        }

        public void Deserializes_previously_serialized_message()
        {
            var data = serializer.Serialize(Msg);

            var result = Should.NotThrow(() => Deserialize(data));

            result.ShouldNotBeNull();
        }

        public void Correctly_deserializes_serialized_object()
        {
            var data = serializer.Serialize(Msg);

            var result = Deserialize(data);

            var typed = result.ShouldBeOfType<TestMessage>();
            typed.Test.ShouldBe(Msg.Test);
        }

        public void Correctly_deserializes_different_types()
        {
            var data = serializer.Serialize(Msg2);
            var result = Deserialize(data);

            var typed = result.ShouldBeOfType<TestMessage2>();
            typed.Test.ShouldBe(Msg2.Test);
        }

        public void Throws_when_trying_to_deserialize_malformed_data()
        {
            var data = serializer.Serialize(Msg);
            Array.Resize(ref data, data.Length / 2);

            Should.Throw<CannotDeserializeMessageException>(() => Deserialize(data));
        }

        public void Honors_data_offset_and_length()
        {
            var data = serializer.Serialize(Msg);
            var pre = new byte[] { 1, 2, 3 };
            var post = new byte[] { 4, 5, 6 };
            var testData = pre.Concat(data).Concat(post).ToArray();

            Should.NotThrow(() => serializer.Deserialize(testData, pre.Length, data.Length));
        }

        private IMessage Deserialize(byte[] data)
        {
            return serializer.Deserialize(data, 0, data.Length);
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete