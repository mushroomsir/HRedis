using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class ServerTests
    {
        private const string ip = "127.0.0.1";
        private const int port = 6380;

        [TestMethod, TestCategory("Server")]
        public void Redis_Command_Info()
        {
            using (var rcClient = new RedisClient(ip, port))
            {
                var info = rcClient.Send(RedisCommand.INFO);
                Debug.Write(info.ToString());
            }
        }
    }
}
