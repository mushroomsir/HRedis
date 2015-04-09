
namespace HRedis
{
    public class RedisSentinelClient : RedisBaseClient
    {
        public event SubscribeEventHandler SubscriptionReceived;
        public RedisSentinelClient(string ip, int port)
            : this(new Configuration()
            {
                Host = ip,
                Port = port,
            })
        {

        }

        public RedisSentinelClient(Configuration configuration)
            : base(configuration)
        {

        }
        public void Subscribe(string channelName)
        {
            Send(RedisCommand.SUBSCRIBE, channelName);
            if (SubscriptionReceived != null)
            {
                Listen(SubscriptionReceived);
            }
        }
        public void PSubscribe(string channelName)
        {
            Send(RedisCommand.PSUBSCRIBE, channelName);
            if (SubscriptionReceived != null)
            {
                Listen(SubscriptionReceived);
            }
        }
    }
}
