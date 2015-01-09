

namespace FD.RedisClient
{
    public class Configuration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DataBase { get; set; }
        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }
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
