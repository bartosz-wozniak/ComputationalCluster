using System.Net.Sockets;

namespace CCluster.Common.Communication
{
    static class TcpClientExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public static bool IsConnected(this TcpClient client)
        {
            return client.Client.IsConnected();
        }
    }
}
