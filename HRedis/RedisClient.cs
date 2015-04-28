
using System;

namespace HRedis
{
    public sealed partial class RedisClient : RedisBaseClient
    {
        internal Action<RedisClient> AutoRelease;

        public IJsonConvert JsonConvert { get; set; }
        public RedisClient(RedisConfiguration configuration)
            : base(configuration)
        {
            JsonConvert = Configuration.JsonConvert ?? new JsonConvert();
        }

        public RedisClient(string ip, int port)
            : this(new RedisConfiguration()
            {

                Host = ip,
                Port = port,
                ReceiveTimeout=20,
                SendTimeout = 20,
            })
        {

        }

        public override void Dispose()
        {
            if (AutoRelease == null)
                base.Dispose();
            else
                AutoRelease(this);
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
