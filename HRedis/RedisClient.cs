
namespace HRedis
{
    public partial class RedisClient : RedisBaseClient
    {
        public RedisClient() : this(new Configuration())
        {
        }

        public RedisClient(string ip, int port)
            : this(new Configuration()
            {
                Host = ip,
                Port = port,
            })
        {

        }
        public RedisClient(Configuration configuration)
            : base(configuration)
        {

        }

    }
}
