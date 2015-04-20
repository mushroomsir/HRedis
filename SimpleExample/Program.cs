using HRedis;

namespace SimpleExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PoolRedisClient prc = new PoolRedisClient("127.0.0.1", 6381);



            RedisClient rc = new RedisClient("127.0.0.1", 6381);
            rc.Info();
        }
    }
}
