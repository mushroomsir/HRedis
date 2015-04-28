

namespace HRedis
{
    public class RedisConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public int SendTimeout { get; set; }

        public int ReceiveTimeout { get; set; }

        public string PassWord { get; set; }
        public IJsonConvert JsonConvert { get; set; }
        public RedisConfiguration()
        {
            Host = "127.0.0.1";
            Port = 6379;
            SendTimeout = -1;
            ReceiveTimeout = -1;
        }
    }

    public class PoolConfiguration : RedisConfiguration
    {
        public int MaxClients { get; set; }

        public PoolConfiguration()
        {
            MaxClients = 100;
        }
    }
}
