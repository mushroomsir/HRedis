
namespace HRedis
{
    public partial class RedisBaseClient
    {
        public string Ping()
        {
            return Execute(RedisCommand.PING).ToString();
        }

        public string Info()
        {
            return Execute(RedisCommand.INFO).ToString();
        }
    }
}
