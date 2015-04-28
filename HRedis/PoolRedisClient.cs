using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HRedis
{
    public class PoolRedisClient : IDisposable
    {
        private PoolConfiguration _configuration;
        private readonly ConcurrentQueue<RedisClient> _pool;
        private readonly List<RedisClient> _usegPool;

        private object obj=new object();
        public PoolRedisClient(PoolConfiguration configuration)
        {
            _configuration = configuration;
            _pool = new ConcurrentQueue<RedisClient>();
            _usegPool=new List<RedisClient>();
        }

        public PoolRedisClient(string ip, int port)
            : this(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
            })
        {

        }

        public object Multi(Func<RedisClient, object> func)
        {
            RedisClient client;
            if (!_pool.TryDequeue(out client))
            {
                client = new RedisClient(_configuration);
                lock (obj)
                {
                    _usegPool.Add(client);
                }
            }
            object result = null;
            try
            {
                result = func(client);
            }
            catch (Exception)
            {
                lock (obj)
                {
                    _usegPool.Remove(client);
                }
                _pool.Enqueue(client);
                throw;
            }
            _pool.Enqueue(client);
            return result;
        }

        public RedisClient Single
        {
            get
            {
                RedisClient client;
                if (!_pool.TryDequeue(out client))
                {
                    client = new RedisClient(_configuration)
                    {
                        AutoRelease = Release
                    };
                    lock (obj)
                    {
                        _usegPool.Add(client);
                    }
                }
                return client;
            }
        }
        private void Release(RedisClient client)
        {
            _pool.Enqueue(client);
            lock (obj)
            {
                _usegPool.Remove(client);
            }
        }
        public void Dispose()
        {
            for (int i = 0; i < _usegPool.Count; i++)
            {
                _usegPool[i].Dispose();
            }
        }
    }
}
