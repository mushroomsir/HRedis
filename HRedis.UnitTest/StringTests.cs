using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class StringTests
    {
        private string ip = MockData.MasterIp;
        private int port = MockData.MasterPort;

        [TestMethod, TestCategory("String")]
        public void Set_Get_key()
        {
            var key = "Set_Get_key";
            using (var rcClient = new RedisClient(ip, port))
            {
                rcClient.Set(key, key);

                var info2 = rcClient.Get(key);

                Assert.AreEqual(info2.ToString(), key);
            }
        }

        [TestMethod, TestCategory("String")]
        public void Del_key()
        {
            using (var rcClient = new RedisClient(ip, port))
            {
                rcClient.Set("Set_Get_key", "Set_Get_key");
                var reply = rcClient.DelKey("Set_Get_key");

                Assert.AreEqual(reply, 1);
            }
        }


        [TestMethod, TestCategory("String")]
        public void Set_key_Expire()
        {
            var key = "Set_key_Expire";
            using (var rcClient = new RedisClient(ip, port))
            {
                rcClient.Set(key, key, 5);

                var info1 = rcClient.Get(key);

                Assert.AreEqual(info1.ToString(), key);

                Thread.Sleep(6000);

                var info2 = rcClient.Get(key);

                Assert.AreEqual(info2, "");
            }
        }
    }
}
