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
                return socket.Connected;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }

    public class RedisSocket
    {
        private Socket socket;
        internal NetworkStream Nstream;
        private Configuration configuration;
        public RedisSocket(Configuration config)
        {
            configuration = config;
        }
        internal void Connect()
        {
            if (socket == null)
                InitSocket();

            else if (!socket.IsConnected())
            {
                Close();
                InitSocket();
            }
            else
                return;

            socket.Connect(configuration.Host, configuration.Port);
            Nstream = new NetworkStream(socket);
        }

        private void InitSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (configuration.SendTimeout > 0)
                socket.SendTimeout = configuration.SendTimeout;

            if (configuration.ReceiveTimeout > 0)
                socket.ReceiveTimeout = configuration.ReceiveTimeout;
        }

        public bool IsConnected()
        {
            return socket.IsConnected();
        }
        internal virtual void Close()
        {
            try
            {
                Nstream.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message + ex.StackTrace);
            }
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message + ex.StackTrace);
            }
            try
            {
                socket.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message + ex.StackTrace);
            }
        }
    }

    
    internal class PoolSocket : IDisposable
    {
        private readonly PoolConfiguration configuration;
        private readonly ConcurrentStack<RedisSocket> pool;

        public PoolSocket(PoolConfiguration config)
        {
            configuration = config;
            pool = new ConcurrentStack<RedisSocket>();
        }

        public void Release(RedisSocket socket)
        {
            pool.Push(socket);
        }
        public void Dispose()
        {
            foreach (var socket in pool)
            {
                socket.Close();
            }
        }

        public RedisSocket Acquire()
        {
            RedisSocket socket;
            if (!pool.TryPop(out socket))
            {
                Add();
                pool.TryPop(out socket);
            }
            else if (!socket.IsConnected())
            {
                socket.Close();
                return Acquire();
            }
            return socket;
        }

        private void Add()
        {
            if (pool.Count > configuration.MaxClients)
                throw new InvalidOperationException("Maximum sockets");
            pool.Push(SocketFactory());
        }

        private RedisSocket SocketFactory()
        {
            return new RedisSocket(configuration);
        }
    }
}
