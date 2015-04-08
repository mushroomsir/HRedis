using System;
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
            if (socket == null)
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
        }

        public void Listen(SubscribeEventHandler func)
        {
            do
            {
                func(ReadData());
                Thread.Sleep(10);
            } while (true);
        }

        internal void SendN(RedisCommand command, params string[] args)
        {
            Connect();
            Nstream = new NetworkStream(socket);
            WriteData(command, args);
        }
        internal object ReadReply()
        {
            return ReadData();
        }
        public object Send(RedisCommand command, params string[] args)
        {
            try
            {
                SendN(command, args);
                return ReadReply();
            }
            catch(Exception ex)
            {
                throw new RedisException(ex.Message);
            }
        }

        private void WriteData(RedisCommand command, string[] args)
        {
            if (Nstream == null)
                throw new Exception("No NetworkStream");

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
            var b = (char)Nstream.ReadByte();
            if (b == MessageFormat.CR)
            {
                Nstream.ReadByte();
                b = (char)Nstream.ReadByte();
            }
           
            switch ((RedisMessage) b)
            {
                case RedisMessage.Error:
                    var errorMessage = ReadLine();
                    throw new RedisException(errorMessage);
                case RedisMessage.Bulk:
                    var size = int.Parse(ReadLine());
                    byte[] data = new byte[size];
                    Nstream.Read(data, 0, size);
                    return Encoding.UTF8.GetString(data);
                case RedisMessage.Int:
                    return ReadLine();
                case RedisMessage.Status:
                    return ReadLine();
                case RedisMessage.MultiBulk:
                    return ReadMultiBulk();
                default:
                    return ReadRawReply();
            }
        }
        public string ReadRawReply()
        {
            byte[] data1 = new byte[10*1024];
            Nstream.Read(data1, 0, data1.Length);
            return Encoding.UTF8.GetString(data1);
        }

        public object[] ReadMultiBulk()
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
            StringBuilder sb = new StringBuilder();
            bool should_break = false;
            while (true)
            {
                int c = Nstream.ReadByte();
                if (c == MessageFormat.CR)
                    should_break = true;
                else if (c == MessageFormat.LF && should_break)
                    break;
                else
                {
                    sb.Append((char) c);
                    should_break = false;
                }
            }
            return sb.ToString();
        }

        private void Close()
        {
            Send(RedisCommand.QUIT);

            if (Nstream != null)
                Nstream.Close();

            if (socket != null)
            {
                socket.Disconnect(false);
                socket.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
