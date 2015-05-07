using System;
using System.Collections.Generic;
using System.Linq;

namespace HRedis
{
    public class PoolManager
    {
        private List<RedisServer> _servers;

        private List<ServerConfig> _serversConfig;

        public PoolManager(List<ServerConfig> serversConfig)
        {
            _servers = new List<RedisServer>();
            foreach (var server in serversConfig)
            {
                _servers.Add(new RedisServer(server));
            }
        }

        public T Multi<T>(Func<RedisClient, T> func)
        {
            var redis = _servers.OrderByDescending(n => n.Age).FirstOrDefault();
            if (redis != null)
            {
                redis.Age += 1;
                return redis.poolRedisClient.Multi(func);
            }
            throw new Exception("no redis server");
        }

        public bool Set(string key, string value)
        {
            return Multi((client) => client.Set(key, value));
        }

        public bool Set<T>(string key, T value)
        {
            return Multi((client) => client.Set<T>(key, value));
        }

        public T Get<T>(string key)
        {
            return Multi((client) => client.Get<T>(key));
        }

        public string Get(string key)
        {
            return Multi((client) => client.Get(key));
        }
        public Dictionary<string, string> Info
        {
            get { return Multi((client) => client.Info); }
        }


        private RedisServer GetReadPool()
        {
           return _servers.OrderByDescending(n => n.Age).FirstOrDefault();
        }
        private RedisServer GetWritePool()
        {
            return _servers.FirstOrDefault(n => n.IsMaster);
        }

    }
    internal class RedisServer
    {
        internal long Age
        {
            get { return Age; }
            set
            {
                if (value >= int.MaxValue)
                    Age = 0;
            }
        }

        internal bool IsMaster { get; set; }

        internal PoolRedisClient poolRedisClient { get; set; }

        public RedisServer(ServerConfig config)
        {
            poolRedisClient = new PoolRedisClient(config);
            IsMaster = config.IsMaster;
        }
    }
}
