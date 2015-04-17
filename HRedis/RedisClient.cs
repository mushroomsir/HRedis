
using System;

namespace HRedis
{
    public sealed partial class RedisClient : RedisBaseClient
    {

        internal Action<RedisClient> ReleaseClient;
        internal RedisClient(RedisConfiguration configuration)
            : base(configuration)
        {

        }

        public RedisClient(string ip, int port)
            : this(new RedisConfiguration()
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
