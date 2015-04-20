
using System;
using System.Collections.Concurrent;

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
        /// <summary>
        ///  if you use this method to get a client，You must release client.
        /// </summary>
        /// <returns></returns>
        public RedisClient GetClient()
        {
            RedisClient client;
            if (!_pool.TryPop(out client))
            {
                Add(() =>
                    new RedisClient(_configuration)
                    {
                        ReleaseClient = Release
                    }
                    );
                return GetClient();
            }
            return client;
        }

        public RedisClient Cmd
        {
            get
            {
                RedisClient client;
                if (!_pool.TryPop(out client))
                {
                    Add(() =>
                        new RedisClient(_configuration)
                        {
                            AutoRelease = Release
                        }
                        );
                    return GetClient();
                }
                return client;
            }
        }

        public object Send(RedisCommand command, params string[] args)
        {
            var reply = Cmd.Send(command, args);
            return reply;
        }
        public object Send(string command, params string[] args)
        {
            var reply = Cmd.Send(command, args);
            return reply;
        }

        public void Dispose()
        {
            foreach (var redisClient in _pool)
            {
                redisClient.ReleaseClient = null;
                redisClient.AutoRelease = null;
                redisClient.Dispose();
            }
        }
        internal void Release(RedisClient client)
        {
            _pool.Push(client);
        }

        private void Add(Func<RedisClient> action)
        {
            _pool.Push(action());
        }
    }
}
