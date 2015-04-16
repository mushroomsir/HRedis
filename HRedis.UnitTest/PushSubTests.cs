using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HRedis.UnitTest
{
    [TestClass]
    public class PushSubTests
    {
        private const string ip = "127.0.0.1";
        private const int port = 6381;


        [TestMethod, TestCategory("PushSub")]
        public void Subscribe_Test()
        {
            using (RedisPubSub rsc = new RedisPubSub(ip, port))
            {
                rsc.SubscriptionReceived += rsc_SubscriptionReceived;
                //rsc.Subscribe("xxxxx");
            }
        }

        [TestMethod, TestCategory("PushSub")]
        public void Publish_Test()
        {
            using (RedisPubSub rsc = new RedisPubSub(ip, port))
            {
               // rsc.Publish("321", "test");
            }
        }

        [TestMethod, TestCategory("PushSub")]
        public void Subscribe_Sentinel_Test()
        {

            using (RedisPubSub rsc = new RedisPubSub("127.0.0.1", 20001))
            {
                rsc.SubscriptionReceived += rsc_SubscriptionReceived;

                //rsc.Subscribe("+sdown");
            }

        }

        [TestMethod, TestCategory("PushSub")]
        public void PSubscribe_Sentinel_Test()
        {
            using (RedisPubSub rsc = new RedisPubSub("127.0.0.1", 20001))
            {
                rsc.SubscriptionReceived += rsc_SubscriptionReceived;
               // rsc.PSubscribe("*");
            }
        }


        private void rsc_SubscriptionReceived(object sender, object args)
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
            sr.UnSubscribe("*");
        }
    }
}
