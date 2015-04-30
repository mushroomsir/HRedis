using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRedis
{
    public class Actions
    {
        private PoolRedisClient _client;

        public Actions(PoolRedisClient client)
        {
            _client = client;
        }

        public bool Set<T>(string key, T value)
        {
            return _client.Multi((client) => client.Set<T>(key, value));
        }

        public T Get<T>(string key)
        {
            return _client.Multi((client) => client.Get<T>(key));
        }

        public string Get(string key)
        {
            return _client.Multi((client) => client.Get<string>(key));
        }

        public Dictionary<string, string> Info
        {
            get { return _client.Multi((client) => client.Info); }
        }
    }
}
