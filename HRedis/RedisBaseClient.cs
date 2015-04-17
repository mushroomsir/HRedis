using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace HRedis
{
    public class RedisBaseClient : IDisposable
    {
        private Socket socket;
        protected  RedisConfiguration configuration;
        public RedisBaseClient(RedisConfiguration config)
        {
            configuration = config;
        }
        public object Send(RedisCommand command, params string[] args)
        {
            SendN(command, args);
            return ReadData();
        }
        protected void SendN(RedisCommand command, params string[] args)
        {
            Connect();
            if (!string.IsNullOrEmpty(configuration.PassWord))
            {
                WriteData(RedisCommand.AUTH, new[] { configuration.PassWord });
                ReadData();
            }
            WriteData(command, args);
        }

        internal void Connect()
        {
            if (socket == null)
                InitSocket();
            else if (!socket.IsConnected())
                Reconnect();
            else
                return;
            socket.Connect(configuration.Host, configuration.Port);
        }

        private void Reconnect()
        {
            Close();
            InitSocket();
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
            var status = socket.IsConnected();
            if (status)
                Send(RedisCommand.QUIT);
            try
            {
                if (status)
                    socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message + ex.StackTrace);
            }
            try
            {
                if (socket != null)
                    socket.Close();
                socket = null;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message + ex.StackTrace);
            }
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

            socket.Send(content);
        }

        protected object ReadData()
        {
            var b = (char) ReadFirstByte();

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
                socket.Receive(data, 0, size, SocketFlags.None);
                return Encoding.UTF8.GetString(data);
            }
            if (b == MessageFormat.ReplyFigure || b == MessageFormat.ReplyStatus)
            {
                return ReadLine();
            }
            if ((b == MessageFormat.ReplyError))
            {
                var errorMessage = ReadLine();
                return "|-1|1|" + errorMessage;
            }
            return "|-1|2|invalid message type";
        }

        private int ReadFirstByte()
        {
            byte[] buffer = new byte[1];
            do
            {
                socket.Receive(buffer, 0, 1, SocketFlags.None);
                if (buffer[0] != MessageFormat.CR && buffer[0] != MessageFormat.LF)
                    break;

            } while (buffer[0] != 0);

            return buffer[0];
        }

        private object[] ReadMultiBulk()
        {
            int count = int.Parse(ReadLine());
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
            byte[] buffer = new byte[1];
            do
            {
                socket.Receive(buffer, 0, 1, SocketFlags.None);
                if (buffer[0] == MessageFormat.CR)
                    continue;
                if (buffer[0] == MessageFormat.LF)
                    break;
                sb.Append((char) buffer[0]);

            } while (buffer[0] != 0);

            return sb.ToString();
        }

        public virtual void Dispose()
        {
            Close();
        }
    }
}
