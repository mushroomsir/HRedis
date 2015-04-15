using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class PoolRedisClientTests
    {
        private const string ip = "127.0.0.1";
        private const int port = 6381;



        [TestMethod, TestCategory("poolRedisclient")]
        public void GetClient_Test()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 100
            });
            using (var client = prc.GetClient())
            {
                client.Set("GetClient_Test", "GetClient_Test");

                var info2 = client.Get("GetClient_Test");

                Assert.AreEqual(info2.ToString(), "GetClient_Test");
            }
            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void GetMaxClient_Test()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 100
            });
            for (int i = 0; i < 100; i++)
            {
                using (var client = prc.GetClient())
                {
                    client.Set("GetMaxClient_Test" + i, "GetMaxClient_Test");

                    var info2 = client.Get("GetMaxClient_Test" + i);

                    Assert.AreEqual(info2.ToString(), "GetMaxClient_Test");
                }
            }
            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Parallel_PoolClient_Test()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 100
            });
            Parallel.For(0, 1000, new ParallelOptions() {MaxDegreeOfParallelism = 100}, (index, item) =>
            {
                using (var client = prc.GetClient())
                {
                    Thread.Sleep(100);
                    client.Set("Parallel_PoolClient_Test" + index, "Parallel_PoolClient_Test");

                    var info2 = client.Get("Parallel_PoolClient_Test" + index);

                    Assert.AreEqual(info2.ToString(), "Parallel_PoolClient_Test");
                }
            });
            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void PoolClient_TimeOut_Test()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 100
            });
            object info2;
            using (var client = prc.GetClient())
            {
                var result = client.Set("PoolClient_TimeOut_Test", "PoolClient_TimeOut_Test");
                Thread.Sleep(15000);
                info2 = client.Get("PoolClient_TimeOut_Test");

            }
            Assert.AreEqual(info2.ToString(), "PoolClient_TimeOut_Test");

            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Thread_PoolClient_Test()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 100
            });
            Parallel.For(0, 1000, new ParallelOptions() {MaxDegreeOfParallelism = 100}, (index, item) =>
            {
                var t = new Thread(() =>
                {
                    Thread.Sleep(1000);
                    object info2;
                    using (var client = prc.GetClient())
                    {
                        client.Set("Parallel_PoolClient_Test" + index, "Parallel_PoolClient_Test");

                       Thread.Sleep(15000);

                        info2 = client.Get("Parallel_PoolClient_Test" + index);
                    }
                    Assert.AreEqual(info2.ToString(), "Parallel_PoolClient_Test");
                });
                t.Start();
            });
            Thread.Sleep(20000);
            prc.Dispose();
        }
    }
}
