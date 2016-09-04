using System.Collections.Generic;
using System.Threading;
using CCluster.Messages;

namespace CCluster.Common.Communication.Messaging
{
    public static class StreamReaderHelper
    {
        /// <remarks>
        /// We don't have a way to reliably test for TCP/IP connection close or end-of-message, so we use this kind of
        /// hack. This class is rather untestable (data must be injected to the stream during the SpinWait).
        /// </remarks>
        public static IReadOnlyList<IMessage> ReadAll(this IMessageStreamReader stream)
        {
            var msgs = new List<IMessage>();
            int waitTime = 1000;
            while (stream.MayHaveMessages)
            {
                var part = stream.ReadAvailable();
                if (part.Count == 0)
                {
                    Thread.SpinWait(waitTime);
                    waitTime *= 2;
                }
                else
                {
                    msgs.AddRange(part);
                }
            }
            return msgs;
        }
    }
}
