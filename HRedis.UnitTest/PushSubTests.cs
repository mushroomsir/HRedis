using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HRedis.UnitTest
{
    [TestClass]
    public class PushSubTests
    {
        private string ip = MockData.MasterIp;
        private const int port = 6379;


        [TestMethod, TestCategory("PushSub")]
        public void Subscribe_Test()
        {
            using (RedisPubSub rsc = new RedisPubSub(ip, port))
            {
                rsc.OnMessage += OnMessage;
                rsc.Subscribe("test");

                Thread.Sleep(10000);
            }
        }

        [TestMethod, TestCategory("PushSub")]
        public void Publish_Test()
        {
            using (RedisPubSub rsc = new RedisPubSub(ip, port))
            {
                rsc.Publish("test", "Publish_Test");
            }
        }


        [TestMethod, TestCategory("PushSub")]
        public void PSubscribe_Sentinel_Test()
        {
            using (RedisPubSub rsc = new RedisPubSub(ip, port))
            {
                rsc.OnMessage += OnMessage;
                rsc.PSubscribe("*");

            }
        }
        [TestMethod, TestCategory("PushSub")]
        public void Subscribe_Error_Test()
        {
            using (RedisPubSub rsc = new RedisPubSub(ip, 10000))
            {
                rsc.OnMessage += OnMessage;
                rsc.PSubscribe("*");
            }
            Thread.Sleep(100000);
        }
        private void OnMessage(object sender, object args)
        {
            if (args is object[])
            {
                var list = args as object[];
                foreach (var o in list)
                {
                    Debug.Write("\r\n" + o.ToString());
                }
            }
            else
            {
                Debug.Write("\r\n" + args.ToString());
            }

            var sr = sender as RedisPubSub;
        }
    }
}
