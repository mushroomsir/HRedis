using System;

namespace HRedis
{
    public sealed class RedisPubSub : RedisBaseClient
    {

        public delegate void SubscribeEventHandler(object sender,object args);

        public event SubscribeEventHandler SubscriptionReceived;
        private volatile Int32 Status = 1;

        public  RedisConfiguration RedisClientConfig { get; set; }
        public RedisPubSub(RedisConfiguration config)
            : base(config)
        {
            RedisClientConfig = new RedisConfiguration()
            {
                Host = config.Host,
                Port = config.Port,
                PassWord = config.PassWord,
                ReceiveTimeout = config.ReceiveTimeout,
                SendTimeout = config.SendTimeout
            };
        }

        public RedisPubSub(string ip, int port)
            : this(new RedisConfiguration()
            {
                Host = ip,
                Port = port,
            })
        {

        }
        void Listen(SubscribeEventHandler func)
        {
            do
            {
                if (SubscriptionReceived != null)
                {
                    func(this,ReadData());
                }
                else
                    ReadData();

            } while (Status == 1);
        }

        public object Publish(string channel, string message)
        {
            return Send(RedisCommand.PUBLISH, channel, message);
        }

        public void Subscribe(string channelName)
        {
            Send(RedisCommand.SUBSCRIBE, channelName);
            Listen(SubscriptionReceived);
        }

        public void UnSubscribe(string channelName)
        {
            Status = 0;
            Send(RedisCommand.UNSUBSCRIBE, channelName);
        }

        public void UnPSubscribe(string channelName)
        {
            Status = 0;
            Send(RedisCommand.PUNSUBSCRIBE, channelName);
        }

        public void PSubscribe(string channelName)
        {
            Send(RedisCommand.PSUBSCRIBE, channelName);

            Listen(SubscriptionReceived);
        }
        public override void Dispose()
        {
            Status = 0;
            base.Dispose();
        }
    }
}
