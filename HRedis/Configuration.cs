

namespace HRedis
{
    public class Configuration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        
        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }

        public Configuration()
        {
            Host = "127.0.0.1";
            Port = 6379;
            SendTimeout = -1;
            ReceiveTimeout = -1;
        }
    }

    public class PoolConfiguration : Configuration
    {
        public int MaxClients { get; set; }

        public PoolConfiguration()
        {
            MaxClients = 100;
        }
    }
}
