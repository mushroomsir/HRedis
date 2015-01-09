using System.Net.Sockets;
using System.Text;

namespace FD.RedisClient
{
    public class RedisBaseClient
    {
        private Configuration configuration;
        private Socket socket;

        private  byte[] ReceiveBuffer = new byte[100000];

        public RedisBaseClient(Configuration config)
        {
            configuration = config;
        }
        public RedisBaseClient() : this(new Configuration())
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
            if (!socket.Connected)
            {
                socket.Disconnect(false);
                socket.Close();
            }
        }

        public object SendCommand(RedisCommand command, params string[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("*{0}\r\n", args != null ? args.Length + 1 : 1);

            var cmd = command.ToString();
            sb.AppendFormat("${0}\r\n{1}\r\n", cmd.Length, cmd);

            if (args != null)
            {
                foreach (var arg in args)
                {
                    sb.AppendFormat("${0}\r\n{1}\r\n", arg.Length, arg);
                }
            }
            byte[] c = Encoding.UTF8.GetBytes(sb.ToString());
            try
            {
                Connect();
                socket.Send(c);
                socket.Receive(ReceiveBuffer);

                socket.Disconnect(false);
                socket.Close();

                return Encoding.UTF8.GetString(ReceiveBuffer);
            }
            catch (SocketException e)
            {
                socket.Disconnect(false);
                socket.Close();
            }
            return null;
        }
    }
}
