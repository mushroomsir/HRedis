using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FD.RedisClient
{
    public class RedisClient : RedisBaseClient
    {
        public RedisClient() : this(new Configuration())
        {

        }

        public RedisClient(Configuration configuration)
            : base(configuration)
        {

        }

        public string Set(string key, string value)
        {
            return base.SendCommand(RedisCommand.SET, key, value);
        }

        public string SetByPipeline(string key, string value, int second)
        {
            base.CreatePipeline();
            base.QueueCommand(RedisCommand.SET, key, value);
            base.QueueCommand(RedisCommand.EXPIRE, key, second.ToString());
            return base.FlushPipeline();
        }
    }
}
