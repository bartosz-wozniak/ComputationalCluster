using System;

namespace CCluster.Common.Communication.Exceptions
{
    /// <summary>
    /// The exception that is thrown when one cannot send a message over the wire.
    /// </summary>
    public class CannotSendMessageException : Exception
    {
        public CannotSendMessageException()
        { }

        public CannotSendMessageException(string message)
            : base(message)
        { }

        public CannotSendMessageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
