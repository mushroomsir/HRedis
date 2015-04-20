using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class NewPoolRedisClientTests
    {

        PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
        {
            Host = MockData.MasterIp,
            Port = MockData.MasterPort,
            MaxClients = 100
        });

        [TestMethod, TestCategory("poolRedisclient")]
        public void GetClient_Test()
        {
            var key = "GetClient_Test";
            prc.Cmd.Set(key, key);

            var info2 = prc.Cmd.Get(key);

            Assert.AreEqual(info2.ToString(), key);
            prc.Dispose();

        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void GetMaxClient_Test()
        {
            var key = "GetMaxClient_Test";
            for (int i = 0; i < 100; i++)
            {
                prc.Cmd.Set(key + i, key);
                var info2 = prc.Cmd.Get(key + i);
                Assert.AreEqual(info2.ToString(), key);
            }
            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Parallel_PoolClient_Test()
        {
            var key = "Parallel_PoolClient_Test";
            Parallel.For(0, 1000, new ParallelOptions() {MaxDegreeOfParallelism = 100}, (index, item) =>
            {
                using (var client = prc.GetClient())
                {
                    Thread.Sleep(100);
                    client.Set(key + index, key);

                    var info2 = client.Get(key + index);

                    Assert.AreEqual(info2.ToString(), key);
                }
            });
            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void PoolClient_TimeOut_Test()
        {
            var key = "PoolClient_TimeOut_Test";
            object info2;
            using (var client = prc.GetClient())
            {
                var result = client.Set(key, key);
                Thread.Sleep(15000);
                info2 = client.Get(key);

            }
            Assert.AreEqual(info2.ToString(), key);

            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Thread_PoolClient_Test()
        {
            var key = "Parallel_PoolClient_Test";
            Parallel.For(0, 1000, new ParallelOptions() {MaxDegreeOfParallelism = 100}, (index, item) =>
            {
                var t = new Thread(() =>
                {
                    Thread.Sleep(1000);
                    object info2;
                    using (var client = prc.GetClient())
                    {
                        client.Set(key + index, key);

                       Thread.Sleep(15000);

                       info2 = client.Get(key + index);
                    }
                    Assert.AreEqual(info2.ToString(), key);
                });
                t.Start();
            });
            Thread.Sleep(20000);
            prc.Dispose();
        }
    }
}
