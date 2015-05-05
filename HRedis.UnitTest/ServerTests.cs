using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class ServerTests
    {
        private string ip = MockData.MasterIp;
        private int port = MockData.MasterPort;


        [TestMethod, TestCategory("Server")]
        public void Info_Test()
        {
            using (var rcClient = new RedisClient(ip, port))
            {
                var info = rcClient.Info;
                foreach (var item in info)
                {
                    Debug.Write(item.Key + ":" + item.Value + "\r\n");
                }
            }
        }

        [TestMethod, TestCategory("Server")]
        public void Ping_Test()
        {
            using (RedisSentinel rsc = new RedisSentinel(ip, port))
            {
                var result = rsc.Ping();

                Assert.IsTrue(result);
            }
        }

        [TestMethod, TestCategory("Server")]
        public void Redis_PassWord()
        {
            using (var rcClient = new RedisClient(new RedisConfiguration()
            {
               Host = ip,
               Port = 6381,
               PassWord = "123465"
            }))
            {
                var info = rcClient.Send(RedisCommand.INFO);
            }
        }
        [TestMethod]
        public void Maxclients()
        {
            for (int i = 0; i < 100; i++)
            {
                RedisSentinel rsc = new RedisSentinel(ip, port);

                var result = rsc.Ping();

                Assert.IsTrue(result);

            }
        }
    }
}
