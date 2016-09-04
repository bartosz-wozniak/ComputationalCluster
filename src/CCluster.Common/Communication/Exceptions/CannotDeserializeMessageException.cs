using System;

namespace CCluster.Common.Communication.Exceptions
{
    public class CannotDeserializeMessageException : Exception
    {
        public CannotDeserializeMessageException(Exception innerException)
            : base("Cannot deserialize message.", innerException)
        { }
    }
}
