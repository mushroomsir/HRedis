using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRedis
{
    public partial class RedisClient
    {
        public object Set(string key, string value)
        {
            return Send(RedisCommand.SET, key, value);
        }

        public object Set(string key, string value, int seconds)
        {
            Send(RedisCommand.MULTI);
            Set(key, value);
            Send(RedisCommand.EXPIRE, key, seconds.ToString());
            return Send(RedisCommand.EXEC);
        }

        public object Get(string key)
        {
            return Send(RedisCommand.GET,key);
        }
    }
}
