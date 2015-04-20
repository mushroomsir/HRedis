
namespace HRedis
{
    public class RedisSentinel : RedisBaseClient
    {
         public RedisSentinel(RedisConfiguration config)
            : base(config)
        {
           
        }

         public RedisSentinel(string ip, int port)
            : this(new RedisConfiguration()
            {
                Host = ip,
                Port = port,
            })
        {

        }
    }
}
