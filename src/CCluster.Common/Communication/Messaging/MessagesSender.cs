using System.Collections.Generic;
using CCluster.Common;
using CCluster.Messages;
using log4net;

namespace CCluster.Common.Communication.Messaging
{
    public class MessagesSender : IMessagesSender
    {
        private static readonly byte[] Separator = new byte[] { Constants.MessageSeparator };

        private readonly IMessageSerializer serializer;

        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        public MessagesSender(IMessageSerializer serializer)
        {
            log.Debug("Create Message Sender");
            this.serializer = serializer;
        }

        public void Send(IReadOnlyList<IMessage> messages, INetworkStream outputStream)
        {
            log.Debug($"Send {messages.Count} message(s) to {outputStream}");
            bool isFirst = true;
            foreach (var msg in messages)
            {
                var serialized = serializer.Serialize(msg);
                if (!isFirst)
                {
                    outputStream.Write(Separator, 0, Separator.Length);
                }
                outputStream.Write(serialized, 0, serialized.Length);
                isFirst = false;
            }
        }

        public void Send(IMessage message, INetworkStream outputStream)
        {
            var serialized = serializer.Serialize(message);
            outputStream.Write(serialized, 0, serialized.Length);
        }
    }
}
