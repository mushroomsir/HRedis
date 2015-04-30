using System;
using HRedis;

namespace SimpleExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {


            //using (RedisClient client = new RedisClient("127.0.0.1", 6381))
            //{
            //    Console.WriteLine(client.Set("key", "value"));
            //    Console.WriteLine(client.Get("key"));
            //}

            //new RedisClient(new RedisConfiguration()
            //{
            //    Host = "127.0.0.1",
            //    Port = 6379,
            //    PassWord = "123456",
            //    ReceiveTimeout = 0,
            //    SendTimeout = 0
            //});

            PoolRedisClient prc = new PoolRedisClient("127.0.0.1", 6381);
            var info = prc.Single.Info;


            //  Console.WriteLine((1 == get1() || 1 == get2()));
            //PoolRedisClient prc = new PoolRedisClient("127.0.0.1", 6381);
            //using (var client = prc.GetClient())
            //{
            //    Console.WriteLine(client.Get("key"));
            //}


            //Console.ReadLine();

            RedisPubSub client = new RedisPubSub("127.0.0.1", 6381);

            client.OnMessage = (sender, arcgs) => Console.WriteLine(arcgs);
            client.OnError = (Exception) => Console.WriteLine(Exception.Message);
            client.Subscribe("123");
            Console.ReadLine();
        }

        static int get1()
        {
            return 1;
        }
        static int get2()
        {
            return 2;
        }
    }
}
