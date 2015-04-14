using System;
using System.Diagnostics;
using System.Text;

namespace HRedis
{
    public class RedisBaseClient : IDisposable
    {
        private RedisSocket socket;

        public RedisBaseClient(RedisSocket RedisSocket)
        {
            socket = RedisSocket;
        }
        public object Send(RedisCommand command, params string[] args)
        {
            SendN(command, args);
            return ReadData();
        }
        protected void SendN(RedisCommand command, params string[] args)
        {
            socket.Connect();
            
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

            socket.Nstream.Write(content, 0, content.Length);
        }

        protected object ReadData()
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
                socket.Nstream.Read(data, 0, size);
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
            while ((c = socket.Nstream.ReadByte()) != -1)
            {
                if (c != MessageFormat.CR && c != MessageFormat.LF)
                    break;
            }
            return c;
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
            int c;
            while ((c = socket.Nstream.ReadByte()) != -1)
            {
                if (c == MessageFormat.CR)
                    continue;
                if (c == MessageFormat.LF)
                    break;
                sb.Append((char)c);
            }
            return sb.ToString();
        }

        protected void Close()
        {
            if (socket.IsConnected())
                Send(RedisCommand.QUIT);
            socket.Close();
        }

        public virtual void Dispose()
        {
            Close();
        }
    }
}
