using System;
using System.Net.Sockets;
using System.Text;

namespace FD.RedisClient
{
    public class RedisBaseClient
    {
        private Configuration configuration;
        private Socket socket;

        private byte[] ReceiveBuffer = new byte[100000];

        public RedisBaseClient(Configuration config)
        {
            configuration = config;
        }

        public RedisBaseClient()
            : this(new Configuration())
        {
        }

        private void Connect()
        {
            if (socket != null && socket.Connected)
                return;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = configuration.NoDelaySocket
            };

            if (configuration.SendTimeout > 0)
                socket.SendTimeout = configuration.SendTimeout;

            if (configuration.ReceiveTimeout > 0)
                socket.ReceiveTimeout = configuration.ReceiveTimeout;

            socket.Connect(configuration.Host, configuration.Port);
            if (socket.Connected)
                return;

            Close();
        }

        public void CreatePipeline()
        {
            SendCommand(RedisCommand.MULTI, new string[] {}, true);
        }

        public string FlushPipeline()
        {
            var result = SendCommand(RedisCommand.EXEC, new string[] {}, true);
            Close();
            return result;
        }

        public string SendCommand(RedisCommand command, params string[] args)
        {
            return SendCommand(command, args,false);
        }

        public string QueueCommand(RedisCommand command, params string[] args)
        {
            return SendCommand(command, args, true);
        }

        public string SendCommand(RedisCommand command, string[] args, bool isPipeline=false)
        {
            //请求头部格式， *<number of arguments>\r\n
            const string headstr = "*{0}\r\n";
            //参数信息       $<number of bytes of argument N>\r\n<argument data>\r\n
            const string bulkstr = "${0}\r\n{1}\r\n";

            var sb = new StringBuilder();
            sb.AppendFormat(headstr, args.Length + 1);

            var cmd = command.ToString();
            sb.AppendFormat(bulkstr, cmd.Length, cmd);

            foreach (var arg in args)
            {
                sb.AppendFormat(bulkstr, arg.Length, arg);
            }
            byte[] c = Encoding.UTF8.GetBytes(sb.ToString());
            try
            {
                Connect();
                socket.Send(c);
                
                socket.Receive(ReceiveBuffer);
                if (!isPipeline)
                {
                    Close();
                }
                return ReadData();
            }
            catch (SocketException e)
            {
                Close();
            }
            return null;
        }

        private string ReadData()
        {
            var data = Encoding.UTF8.GetString(ReceiveBuffer);
            char c = data[0];
            //错误消息检查。
            if (c == '-') //异常处理。
                throw new Exception(data);
            //状态回复。
            if (c == '+')
                return data;
            return data;
        }

        public string Set(string key, string value)
        {
           return this.SendCommand(RedisCommand.SET, key, value);
        }

        public string SetByPipeline(string key, string value, int second)
        {
            this.CreatePipeline();
            this.QueueCommand(RedisCommand.SET, key, value);
            this.QueueCommand(RedisCommand.EXPIRE, key, second.ToString());
            return this.FlushPipeline();
        }


        /// <summary>
        /// 关闭client
        /// </summary>
        public void Close()
        {
            socket.Disconnect(false);
            socket.Close();
        }
    }
}
