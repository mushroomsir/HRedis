
using System;
using System.Collections.Concurrent;

namespace HRedis
{
    public class PoolRedisClient : IDisposable
    {
        private PoolSocket _poolSocketpool;
        private PoolConfiguration _configuration;
        private readonly ConcurrentStack<RedisClient> _pool;

        public PoolRedisClient(PoolConfiguration configuration)
        {
            _configuration = configuration;
            _poolSocketpool = new PoolSocket(configuration);
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
            return Acquire();
        }

        public void Dispose()
        {
            foreach (var redisClient in _pool)
            {
                redisClient.ReleaseClient = null;
                redisClient.Dispose();
            }
        }

        public void Release(RedisClient socket)
        {
            _pool.Push(socket);
        }

        private RedisClient Acquire()
        {
            RedisClient client;
            if (!_pool.TryPop(out client))
            {
                Add();
                _pool.TryPop(out client);
            }
            return client;
        }

        private void Add()
        {
            if (_pool.Count > _configuration.MaxClients)
                throw new InvalidOperationException("Maximum sockets");
            _pool.Push(SocketFactory());
        }

        private RedisClient SocketFactory()
        {
            return new RedisClient(_poolSocketpool.Acquire())
            {
                ReleaseClient = Release
            };
        }
    }
}
