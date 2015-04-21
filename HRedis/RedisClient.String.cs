
using System;

namespace HRedis
{
    public partial class RedisClient
    {
        public object Set(string key, string value)
        {
            return Execute(RedisCommand.SET, key, value);
        }

        public object Set(string key, string value, int seconds)
        {
            Send(RedisCommand.MULTI);
            Set(key, value);
            Send(RedisCommand.EXPIRE, key, seconds.ToString());
            return Execute(RedisCommand.EXEC);
        }

        public object Get(string key)
        {
            return Execute(RedisCommand.GET, key);
        }

      
    }
}
