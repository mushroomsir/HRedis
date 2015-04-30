
using System;

namespace HRedis
{
    public class RedisSentinel:IDisposable
    {
        private RedisClient client;

        public RedisSentinel(RedisConfiguration config)
        {
            client = new RedisClient(config);
        }

        public RedisSentinel(string ip, int port)
            : this(new RedisConfiguration()
            {
                Host = ip,
                Port = port,
            })
        {

        }

        public bool Ping()
        {
            try
            {
                return client.Ping();
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool PingRaw()
        {
            return client.Ping();
        }
        public void Dispose()
        {
            client.Dispose();
        }
    }
}
