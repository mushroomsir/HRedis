using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class SentinelTests
    {
        private const string ip = "127.0.0.1";
        private const int port = 20002;

        [TestMethod,TestCategory("sentinel")]
        public void PSubscribe_Test()
        {
            using (RedisSentinelClient rsc = new RedisSentinelClient(ip, port))
            {
                rsc.SubscriptionReceived += rsc_SubscriptionReceived;
                rsc.PSubscribe("*");
            }
        }
        void rsc_SubscriptionReceived(object message)
        {
            if (message is object[])
            {
                var list = message as object[];
                foreach (var o in list)
                {
                    Debug.Write("\r\n" + o.ToString());
                }
            }
            else
            {
                Debug.Write("\r\n" + message.ToString());
            }
        }
    }
}
