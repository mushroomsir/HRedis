using System;
using System.Collections.Concurrent;
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

                if (!socket.Poll(1, SelectMode.SelectWrite))
                    return false;;
                
                return true;
            }
            catch (SocketException exception)
            {
                Debug.Write(exception.Message);
            }
            return false;
        }
    }
}
