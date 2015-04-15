
using System;

namespace HRedis
{
    public sealed partial class RedisClient : RedisBaseClient
    {

        internal Action<RedisClient> ReleaseClient;
        internal RedisClient(Configuration redisSocket)
            : base(redisSocket)
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
