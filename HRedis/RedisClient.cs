
using System;

namespace HRedis
{
    public sealed partial class RedisClient : RedisBaseClient
    {
        internal Action<RedisClient> ReleaseClient;
        internal Action<RedisClient> AutoRelease;
        public RedisClient(RedisConfiguration configuration)
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
            if (ReleaseClient == null && AutoRelease == null)
                base.Dispose();
            else if (AutoRelease == null)
                ReleaseClient(this);
        }

        protected override void Continuation()
        {
            if (AutoRelease != null)
                AutoRelease(this);
        }

        public int DelKey(string key)
        {
            int nums;
            var reply = Execute(RedisCommand.DEL, key).ToString();
            if (int.TryParse(reply, out nums))
                return nums;
            throw new Exception(reply);
        }
    }
}
