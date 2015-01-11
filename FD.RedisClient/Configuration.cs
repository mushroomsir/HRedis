

namespace FD.RedisClient
{
    public class Configuration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        
        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }

        /// <summary>
        /// Socket 是否正在使用 Nagle 算法。
        /// </summary>
        public bool NoDelaySocket { get; set; }

        public Configuration()
        {
            Host = "localhost";
            Port = 6379;
            SendTimeout = -1;
            NoDelaySocket = false;
        }
    }
}
