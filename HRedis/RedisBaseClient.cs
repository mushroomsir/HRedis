using System;
using System.ComponentModel.Design;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HRedis
{
    public class RedisBaseClient : IDisposable
    {
        private Configuration configuration;
        private Socket socket;
        private NetworkStream Nstream;

    
        public delegate void SubscribeEventHandler(object message);
        public RedisBaseClient(Configuration config)
        {
            configuration = config;
        }


        private void Connect()
        {
            if (socket != null && socket.Connected)
                return;
            if (socket == null || !socket.Connected)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = configuration.NoDelaySocket
                };

                if (configuration.SendTimeout > 0)
                    socket.SendTimeout = configuration.SendTimeout;

                if (configuration.ReceiveTimeout > 0)
                    socket.ReceiveTimeout = configuration.ReceiveTimeout;
            }
            socket.Connect(configuration.Host, configuration.Port);
            Nstream = new NetworkStream(socket);
        }


        public object Send(RedisCommand command, params string[] args)
        {
            SendN(command, args);
            return ReadData();
        }

        protected void SendN(RedisCommand command, params string[] args)
        {
            Connect();
            
            WriteData(command, args);
        }
        private void WriteData(RedisCommand command, string[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(MessageFormat.Head, args.Length + 1);

            var cmd = command.ToString();
            sb.AppendFormat(MessageFormat.Argument, cmd.Length, cmd);

            foreach (var arg in args)
            {
                sb.AppendFormat(MessageFormat.Argument, arg.Length, arg);
            }
            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

            Nstream.Write(content, 0, content.Length);
        }

        private object ReadData()
        {
            var b = (char)ReadFirstByte();
           
            if (b == MessageFormat.ReplyMultiBulk)
            {
               return ReadMultiBulk();
            }
            if (b == MessageFormat.ReplyBulk)
            {
                var size = int.Parse(ReadLine());
                if (size == -1)
                    return null;
                byte[] data = new byte[size];
                Nstream.Read(data, 0, size);
                return Encoding.UTF8.GetString(data);
            }
            if (b == MessageFormat.ReplyFigure || b == MessageFormat.ReplyStatus)
            {
                return ReadLine();
            }
            if ((b == MessageFormat.ReplyError))
            {
                var errorMessage = ReadLine();
                throw new RedisException("redis message:"+errorMessage);
            }
            throw new RedisException("invalid message type");
        }
        int ReadFirstByte()
        {
            int c;
            while ((c = Nstream.ReadByte()) != -1)
            {
                if (c != MessageFormat.CR && c != MessageFormat.LF)
                    break;
            }
            return c;
        }
        private object[] ReadMultiBulk()
        {
            int count = int.Parse(ReadLine()); ;
            if (count == -1)
                return null;

            object[] lines = new object[count];
            for (int i = 0; i < count; i++)
            {
                lines[i] = ReadData();
            }
            return lines;
        }
        private string ReadLine()
        {
            var sb = new StringBuilder();
            int c;
            while ((c = Nstream.ReadByte()) != -1)
            {
                if (c == MessageFormat.CR)
                    continue;
                if (c == MessageFormat.LF)
                    break;
                sb.Append((char)c);
            }
            return sb.ToString();
        }
        protected void Listen(SubscribeEventHandler func)
        {
            do
            {
                func(ReadData());
                Thread.Sleep(10);
            } while (true);
        }

        private void Close()
        {
            SendN(RedisCommand.QUIT);
            if (Nstream != null)
                Nstream.Close();
         
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Send);
                socket.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
