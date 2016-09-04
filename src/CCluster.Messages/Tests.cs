using System;
using System.ComponentModel;

namespace CCluster.Messages
{
    /// <summary>
    /// For testing purposes only.
    /// </summary>
    [Obsolete("Use only for testing purposes.")]
    public class TestMessage : IMessage
    {
        public string Test { get; set; }
    }

    /// <summary>
    /// For testing purposes only.
    /// </summary>
    [Obsolete("Use only for testing purposes.")]
    public class TestMessage2 : IMessage
    {
        public int Test { get; set; }
    }
}
