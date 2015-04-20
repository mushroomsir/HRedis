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
                for (int i = 0; i < 10000; i++)
                {
                    rcClient.Set("Continuation_Set_Get" + i, i.ToString(), 10);
                    rcClient.Get("Continuation_Set_Get" + i);
                    rcClient.Send(RedisCommand.INFO);
                }
            }
        }

        [TestMethod, TestCategory("Performance")]
        public void Continuation_Set_Get_ByDifferentClient()
        {
            for (int i = 0; i < 10000; i++)
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
