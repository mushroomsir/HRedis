
namespace HRedis
{
    public partial class RedisBaseClient
    {
        public string Ping()
        {
            return Send(RedisCommand.PING).ToString();
        }
        public string Info()
        {
            return Send(RedisCommand.INFO).ToString();
        }
    }
}
