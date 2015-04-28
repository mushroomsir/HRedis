
using System;

namespace HRedis
{
    public partial class RedisClient
    {
        public bool Set<T>(string key, T value)
        {
            return Execute(RedisCommand.SET, key, JsonConvert.SerializeObject(value)) == ReplyFormat.ReplySuccess;
        }

        public bool Set<T>(string key, T value, int seconds)
        {
            Send(RedisCommand.MULTI);
            Set(key, JsonConvert.SerializeObject(value));
            Send(RedisCommand.EXPIRE, key, seconds.ToString());
            return Execute(RedisCommand.EXEC) == ReplyFormat.ReplySuccess;
        }
      
        public string Set(string key, string value)
        {
            return Execute(RedisCommand.SET, key);
        }
        public string Get(string key)
        {
            return Execute(RedisCommand.GET, key);
        }

        public T Get<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(Execute(RedisCommand.GET, key));
        }

      
    }
}
