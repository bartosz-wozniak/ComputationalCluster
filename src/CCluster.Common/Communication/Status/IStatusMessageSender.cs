using System;

namespace CCluster.Common.Communication.Status
{
    /// <summary>
    /// Periodically sends a status message to the CS.
    /// </summary>
    public interface IStatusMessageSender
    {
        TimeSpan Timeout { get; set; }

        void SendIfRequired();
    }
}
