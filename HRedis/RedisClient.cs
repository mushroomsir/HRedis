
using System;

namespace HRedis
{
    public sealed partial class RedisClient : RedisBaseClient
    {
        public IJsonConvert JsonConvert { get; set; }
        public RedisClient(RedisConfiguration configuration)
            : base(configuration)
        {
            JsonConvert = Configuration.JsonConvert ?? new JsonConvert();
            _configuration = configuration;
        }
        private RedisConfiguration _configuration;

        public RedisClient(string ip, int port)
            : this(new RedisConfiguration()
            {

                Host = ip,
                Port = port,
                ReceiveTimeout=60,
                SendTimeout = 60,
            })
        {

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
