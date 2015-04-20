using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class SentinelTests
    {
        private string ip = MockData.SentinelIp;
        private  int port = MockData.SentinelPort;

        [TestMethod, TestCategory("Sentinel")]
        public void Disconnect_Test()
        {
            using (var rcClient = new RedisSentinel(ip, 20002))
            {
                var info = rcClient.Ping();
                Debug.Write(info.ToString());
            }
        }
    }
}
