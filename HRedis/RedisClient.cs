
namespace HRedis
{
    public partial class RedisClient : RedisBaseClient
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
    }
}
