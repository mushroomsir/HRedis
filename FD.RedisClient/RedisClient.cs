using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRedis
{
    public class RedisClient : RedisBaseClient
    {
        public RedisClient() : this(new Configuration())
        {

        }

        public event SubscribeEventHandler SubscriptionReceived;

        public RedisClient(string ip,int port)
            : this(new Configuration()
            {
                 Host=ip,
                 Port = port,
            })
        {

        }
        public RedisClient(Configuration configuration)
            : base(configuration)
        {

        }

        public void Subscribe(string channelName)
        {
            Send(RedisCommand.SUBSCRIBE);

            if (SubscriptionReceived != null)
            {
                Listen(SubscriptionReceived);
            }
        }

        public object Set(string key, string value)
        {
            return Send(RedisCommand.SET, key, value);
        }
        #region Pipeline

        public void CreatePipeline()
        {
            Send(RedisCommand.MULTI);
        }

        public object QueueCommand(RedisCommand command, params string[] args)
        {
            return Send(command, args);
        }

        public object FlushPipeline()
        {
            var result = Send(RedisCommand.EXEC, new string[] { });
            return result;
        }
      
        public object SetByPipeline(string key, string value, int second)
        {
            CreatePipeline();
            QueueCommand(RedisCommand.SET, key, value);
            QueueCommand(RedisCommand.EXPIRE, key, second.ToString());
            return FlushPipeline();
        }
        #endregion
    }
}
