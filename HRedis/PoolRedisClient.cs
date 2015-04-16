
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;

namespace HRedis
{
    public class PoolRedisClient : IDisposable
    {
        private PoolConfiguration _configuration;
        private readonly ConcurrentStack<RedisClient> _pool;

        public PoolRedisClient(PoolConfiguration configuration)
        {
            _configuration = configuration;
            _pool = new ConcurrentStack<RedisClient>();
        }

        public PoolRedisClient(string ip, int port)
            : this(new PoolConfiguration()
            {

                Host = ip,
                Port = port,
            })
        {

        }

        public RedisClient GetClient()
        {
            RedisClient client;
            if (!_pool.TryPop(out client))
            {
                Add();
                return GetClient();
            }
            return client;
        }

        public void Dispose()
        {
            foreach (var redisClient in _pool)
            {
                redisClient.ReleaseClient = null;
                redisClient.Dispose();
            }
        }

        internal void Release(RedisClient client)
        {
            _pool.Push(client);
        }

        private void Add()
        {
            _pool.Push(ClientFactory());
        }
        private RedisClient ClientFactory()
        {
            return new RedisClient(_configuration)
            {
                ReleaseClient = Release
            };
        }
    }
}
