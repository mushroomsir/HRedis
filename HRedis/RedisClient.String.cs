
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
            Execute(RedisCommand.MULTI);
            Execute(RedisCommand.SET, key, JsonConvert.SerializeObject(value));
            Execute(RedisCommand.EXPIRE, key, seconds.ToString());
            var reply = Execute(RedisCommand.EXEC) as object[];

            return reply[0].ToString() == ReplyFormat.ReplySuccess;
        }

        public T Get<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(Execute(RedisCommand.GET, key).ToString());
        }


        public bool Set(string key, string value)
        {
            return Execute(RedisCommand.SET, key, value) == ReplyFormat.ReplySuccess;
        }

        public bool Set(string key, string value, int seconds)
        {
            Execute(RedisCommand.MULTI);
            Execute(RedisCommand.SET, key, value);
            Execute(RedisCommand.EXPIRE, key, seconds.ToString());
            var reply = Execute(RedisCommand.EXEC) as object[];

            return reply[0].ToString() == ReplyFormat.ReplySuccess;
        }

        public string Get(string key)
        {
            var reply = Execute(RedisCommand.GET, key);
            return reply == null ? "" : reply.ToString();
        }
    }
}
