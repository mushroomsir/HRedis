
using System;

namespace HRedis
{
    public partial class RedisClient
    {
        public object Set(string key, string value)
        {
            return Execute(() => Send(RedisCommand.SET, key, value));
        }

        public object Set(string key, string value, int seconds)
        {
            Send(RedisCommand.MULTI);
            Set(key, value);
            Send(RedisCommand.EXPIRE, key, seconds.ToString());
            return Execute(() => Send(RedisCommand.EXEC));
        }

        public object Get(string key)
        {
            return Execute(() => Send(RedisCommand.GET, key));
        }

        private object Execute(Func<object> func)
        {
            var reply = func();
            Continuation();
            return reply;
        }
    }
}
