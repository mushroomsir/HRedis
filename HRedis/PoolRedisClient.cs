using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace HRedis
{
    public class PoolRedisClient : IDisposable
    {
        private PoolConfiguration _configuration;
        private readonly ConcurrentQueue<RedisClient> _pool;

        private Actions action;

        private SemaphoreSlim _semaphore;

        public PoolRedisClient(PoolConfiguration configuration)
        {
            _configuration = configuration;
            _pool = new ConcurrentQueue<RedisClient>();
            _semaphore = new SemaphoreSlim(configuration.MinClients, configuration.MaxClients);

            action = new Actions(this);
        }

        public PoolRedisClient(string ip, int port)
            : this(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MinClients = 10,
                MaxClients = 100
            })
        {

        }

        public T Multi<T>(Func<RedisClient, T> func)
        {
            _semaphore.Wait(60000);

            RedisClient client = null;
            try
            {
                if (!_pool.TryDequeue(out client))
                {
                    client = new RedisClient(_configuration);
                }
                var result = func(client);
                _pool.Enqueue(client);
                return result;
            }
            catch (SocketException exception)
            {
                if (client != null)
                    client.Dispose();
                throw;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Actions Single
        {
            get { return action; }
        }

        public void Dispose()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                RedisClient client;
                if (_pool.TryDequeue(out client))
                    client.Dispose();
            }
        }
    }
}
