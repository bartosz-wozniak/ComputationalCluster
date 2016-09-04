using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using CCluster.Common.Communication;
using CCluster.Common.Communication.Exceptions;
using CCluster.Common.Communication.Messaging;
using CCluster.Common.Configuration;
using CCluster.Messages;
using FakeItEasy;
using MediatR;
using Shouldly;

namespace CCluster.Common.Tests.Communication
{
    public class ServerClientTests
    {
        private readonly ITcpClient tcpClient;
        private readonly INetworkStream stream;
        private readonly IMessageStreamReader reader;
        private readonly IMessagesSender sender;
        private readonly ICsConfiguration config;
        private readonly IMediator mediator;

        private readonly IServerClient serverClient;

        private bool messagesRead = false;
        private bool tcpClientCreated = false;
        private bool readerCreated = false;

        public ServerClientTests()
        {
            tcpClient = A.Fake<ITcpClient>();
            stream = A.Fake<INetworkStream>();
            reader = A.Fake<IMessageStreamReader>();
            sender = A.Fake<IMessagesSender>();
            config = A.Fake<ICsConfiguration>();
            mediator = A.Fake<IMediator>();

            A.CallTo(() => config.Address).Returns(IPAddress.Loopback);
            A.CallTo(() => config.Port).Returns(1234);
            A.CallTo(() => tcpClient.GetStream()).Returns(stream);
            A.CallTo(() => reader.MayHaveMessages).ReturnsLazily(() => messagesRead = !messagesRead);
            A.CallTo(() => reader.ReadAvailable()).ReturnsLazily(() => A.CollectionOfFake<IMessage>(1).ToArray());

            Func<ITcpClient> clientFactory = () =>
            {
                tcpClientCreated = true;
                return tcpClient;
            };
            Func<INetworkStream, IMessageStreamReader> readerFactory = s =>
            {
                if (s == stream)
                {
                    readerCreated = true;
                    return reader;
                }
                throw new InvalidOperationException();
            };

            serverClient = new ServerClient(config, sender, mediator, clientFactory, readerFactory);
        }

        public void Creates_new_TcpClient()
        {
            SendFake();

            tcpClientCreated.ShouldBeTrue();
        }

        public void Connects_the_TcpClient_to_the_correct_endpoint()
        {
            SendFake();

            A.CallTo(() => tcpClient.Connect(config.Address, config.Port))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Creates_the_Reader_with_correct_stream()
        {
            SendFake();

            readerCreated.ShouldBeTrue();
        }

        public void Sends_the_message_using_dedicated_sender()
        {
            var msg = SendFake();

            A.CallTo(() => sender.Send(A<IReadOnlyList<IMessage>>.That.IsSameSequenceAs(new[] { msg }), stream))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Reads_response()
        {
            SendFake();

            A.CallTo(() => reader.ReadAvailable()).MustHaveHappened();
        }

        public void Puts_parsed_messages_in_the_bus_using()
        {
            var response = new TestMessage();
            A.CallTo(() => reader.ReadAvailable()).Returns(new[] { response });

            SendFake();

            A.CallTo(() => mediator.Send(response)).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void When_sending_no_response_message_It_does_not_try_to_parse_response()
        {
            SendFake(true);

            A.CallTo(reader).MustNotHaveHappened();
            A.CallTo(mediator).MustNotHaveHappened();
        }

        public void Wraps_exceptions_during_connection()
        {
            var innerException = new SocketException();
            A.CallTo(() => tcpClient.Connect(config.Address, config.Port)).Throws(innerException);

            var ex = Should.Throw<CannotSendMessageException>(() => SendFake());
            ex.InnerException.ShouldBeSameAs(innerException);
        }

        public void Wraps_exceptions_during_send()
        {
            var innerException = new SocketException();
            A.CallTo(() => sender.Send(new IMessage[0], null)).WithAnyArguments().Throws(innerException);

            var ex = Should.Throw<CannotSendMessageException>(() => SendFake());
            ex.InnerException.ShouldBeSameAs(innerException);
        }

        public void Wraps_exceptions_during_read()
        {
            var innerException = new SocketException();
            A.CallTo(() => reader.ReadAvailable()).Throws(innerException);

            var ex = Should.Throw<CannotSendMessageException>(() => SendFake());
            ex.InnerException.ShouldBeSameAs(innerException);
        }

        public void Throws_NoResponseException_when_server_disconnects_before_sending_response()
        {
            A.CallTo(() => reader.ReadAvailable()).Returns(new IMessage[0]);

            Should.Throw<NoResponseException>(() => SendFake());
        }

        public void Disposes_TcpClient_at_the_end_of_the_method()
        {
            SendFake();

            A.CallTo(() => tcpClient.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void Disposes_the_network_stream_after_the_message_is_sent()
        {
            SendFake();

            A.CallTo(() => stream.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        public void TcpClient_methods_must_have_been_called_in_correct_order()
        {
            SendFake();

            var seq = A.SequentialCallContext();
            A.CallTo(() => tcpClient.Connect(config.Address, config.Port))
                .MustHaveHappened()
                .InOrder(seq);
            A.CallTo(() => tcpClient.GetStream())
                .MustHaveHappened()
                .InOrder(seq);
            A.CallTo(() => stream.Dispose())
                .MustHaveHappened()
                .InOrder(seq);
            A.CallTo(() => tcpClient.Dispose())
                .MustHaveHappened()
                .InOrder(seq);
        }

        private IMessage SendFake(bool noResponse = false)
        {
            var msg = noResponse ? A.Fake<INoResponseMessage>() : A.Fake<IMessage>();
            serverClient.Send(msg);
            return msg;
        }

        private sealed class TestMessage : IMessage
        { }
    }
}
