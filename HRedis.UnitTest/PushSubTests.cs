using System;
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
        public void UnSubscribe_Test()
        {
            RedisPubSub rsc = new RedisPubSub(ip, port);
            rsc.OnUnSubscribe += (obj) =>
            {
                var reply = obj as object[];

                Debug.WriteLine(reply[0].ToString() + reply[1].ToString() + reply[2].ToString());
            };
            rsc.OnSuccess += (obj) =>
            {
                Debug.WriteLine(obj[0].ToString() + obj[1].ToString() + obj[2].ToString());
            };

            rsc.Subscribe("test");
            Thread.Sleep(1000);
            rsc.UnSubscribe("test");
            rsc.Subscribe("test");
            Thread.Sleep(1000);
            rsc.UnSubscribe("test");
        }


        [TestMethod]
        public void Push_Sentinel_Test()
        {
            using (RedisPubSub rcClient = new RedisPubSub("127.0.0.1", 20002))
            {
                rcClient.Publish("*", "test");
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
            Thread.Sleep(5000);
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
