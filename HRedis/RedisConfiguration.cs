

namespace HRedis
{
    public class RedisConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        /// <summary>
        /// Seconds
        /// </summary>
        public int SendTimeout { get; set; }
        /// <summary>
        /// Seconds
        /// </summary>
        public int ReceiveTimeout { get; set; }

        public string PassWord { get; set; }
        public IJsonConvert JsonConvert { get; set; }
        public RedisConfig()
        {
            Host = "127.0.0.1";
            Port = 6379;
            SendTimeout = 60;
            ReceiveTimeout = 60;
        }
    }

    public class PoolConfig : RedisConfig
    {
        public int MaxClients { get; set; }
        public int MinClients { get; set; }
        public PoolConfig()
        {
            MaxClients = 100;
            MinClients = 10;
        }
    }

    public class ServerConfig : PoolConfig
    {
        public bool IsMaster { get; set; }
    }
}
