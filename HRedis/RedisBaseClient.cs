using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace HRedis
{
    public partial class RedisBaseClient : IDisposable
    {
        private Socket socket;
        public readonly RedisConfiguration Configuration;

        public RedisBaseClient(RedisConfiguration config)
        {
            Configuration = config;
        }

        internal object Send(RedisCommand command, params string[] args)
        {
            SendN(command.ToString(), args);
            return ReadData();
        }

        internal object Send(string command, params string[] args)
        {
            SendN(command, args);
            return ReadData();
        }

        internal void SendN(string command, params string[] args)
        {
            Connect();
            if (!string.IsNullOrEmpty(Configuration.PassWord))
            {
                WriteData(RedisCommand.AUTH.ToString(), new[] {Configuration.PassWord});
                ReadData();
            }
            WriteData(command, args);
        }


        #region Manager Connect 

        private void Connect()
        {
            if (socket == null)
                InitSocket();
            else if (!socket.IsConnected())
                Reconnect();
            else
                return;
            socket.Connect(Configuration.Host, Configuration.Port);

        }

        private void Reconnect()
        {
            Close();
            InitSocket();
        }

        private void InitSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (Configuration.SendTimeout > 0)
                socket.SendTimeout = Configuration.SendTimeout;

            if (Configuration.ReceiveTimeout > 0)
                socket.ReceiveTimeout = Configuration.ReceiveTimeout;
        }

        private void Close()
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

        #endregion

        #region  Reading and Writing for Messages

        private void WriteData(string command, string[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(MessageFormat.Head, args.Length + 1);

            var cmd = command;
            sb.AppendFormat(MessageFormat.Argument, cmd.Length, cmd);

            foreach (var arg in args)
            {
                sb.AppendFormat(MessageFormat.Argument, arg.Length, arg);
            }
            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

            socket.Send(content);
        }

        internal object ReadData()
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
                return errorMessage;
            }
            return b;
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

        #endregion

        protected virtual void Continuation()
        {

        }

        private object Execute(Func<object> func, Action action)
        {
            object reply = null;
            try
            {
                reply = func();
            }
            catch (Exception)
            {
                action();
                throw;
            }
            return reply;
        }

        public string Execute(RedisCommand rc, params string[] args)
        {
            return Execute(() => Send(rc, args), Continuation).ToString();
        }
        public virtual void Dispose()
        {
            Close();
        }
    }
}
