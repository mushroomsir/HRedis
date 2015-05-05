using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class PerformanceTests
    {
        private string ip = MockData.MasterIp;
        private int port = MockData.MasterPort;

        [TestMethod, TestCategory("Performance")]
        public void Continuation_Set_Get_BySameClient()
        {
            using (var rcClient = new RedisClient(ip, port))
            {
                for (int i = 0; i < 1000; i++)
                {
                    
                    var reply1=rcClient.Set("Continuation_Set_Get" + i, i.ToString(), 10000);
                    Assert.IsTrue(reply1);
                    var reply=rcClient.Get("Continuation_Set_Get" + i);
                    rcClient.Send(RedisCommand.INFO);
                    Assert.AreEqual(i.ToString(), reply);

                }
            }
        }

        [TestMethod, TestCategory("Performance")]
        public void Continuation_Set_Get_ByDifferentClient()
        {
            for (int i = 0; i < 1000; i++)
            {
                using (var rcClient = new RedisClient(ip, port))
                {
                    rcClient.Set("Continuation_Set_Get_ByDifferentClient" + i, i.ToString(), 10);
                    rcClient.Get("Continuation_Set_Get_ByDifferentClient" + i);
                    rcClient.Send(RedisCommand.INFO);
                }
            }
        }
    }

}
