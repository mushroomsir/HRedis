using System.Diagnostics;
using System.Net.Sockets;

namespace HRedis
{
    internal static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                if (!socket.Connected)
                    return false;

                return !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException exception)
            {
                Debug.Write(exception.Message);
            }
            return false;
        }
    }
}
