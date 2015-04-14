
using System;

namespace HRedis
{
    public sealed partial class RedisClient : RedisBaseClient
    {

        internal Action<RedisClient> ReleaseClient;
        internal RedisClient(RedisSocket redisSocket)
            : base(redisSocket)
        {

        }

        public RedisClient(Configuration configuration)
            : this(new RedisSocket(configuration))
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
        public override void Dispose()
        {
            if (ReleaseClient == null)
                base.Dispose();
            else
                ReleaseClient(this);
        }
    }
}
