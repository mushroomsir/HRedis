using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class StringTests
    {
        private const string ip = "127.0.0.1";
        private const int port = 6381;

        [TestMethod, TestCategory("String")]
        public void Set_Get_key()
        {
            using (var rcClient = new RedisClient(ip, port))
            {
                rcClient.Set("Set_Get_key", "Set_Get_key");

                var info2 = rcClient.Get("Set_Get_key");

                Assert.AreEqual(info2.ToString(), "Set_Get_key");
            }
        }

        [TestMethod, TestCategory("String")]
        public void Set_key_Expire()
        {
            using (var rcClient = new RedisClient(ip, port))
            {
                rcClient.Set("Set_key_Expire", "Set_key_Expire", 10);

                var info1 = rcClient.Get("Set_key_Expire");

                Assert.AreEqual(info1.ToString(), "Set_key_Expire");

                Thread.Sleep(11000);
                var info2 = rcClient.Get("Set_key_Expire");

                Assert.AreEqual(info2, null);

            }
        }
        

    }
}
