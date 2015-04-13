
namespace HRedis
{
    public class RedisSentinelClient : RedisBaseClient
    {
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
      
    }
}
