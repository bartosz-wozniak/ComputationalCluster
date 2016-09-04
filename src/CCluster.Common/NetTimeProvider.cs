using System;

namespace CCluster.Common
{
    public class NetTimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}
