
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
           return client.Ping();
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
