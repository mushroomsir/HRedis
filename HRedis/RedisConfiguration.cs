

namespace HRedis
{
    public class RedisConfiguration
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
        public RedisConfiguration()
        {
            Host = "127.0.0.1";
            Port = 6379;
            SendTimeout = 60;
            ReceiveTimeout = 60;
        }
    }

    public class PoolConfiguration : RedisConfiguration
    {
        public int MaxClients { get; set; }
        public int MinClients { get; set; }
        public PoolConfiguration()
        {
            MaxClients = 100;
        }
    }
}
