using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRedis.UnitTest
{
    [TestClass]
    public class PoolRedisClientTests
    {

        private string ip = MockData.MasterIp;
        private int port = MockData.MasterPort;

        [TestMethod, TestCategory("poolRedisclient")]
        public void Pool_Single()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 100
            });

            prc.Single.Set("GetClient_Test", "GetClient_Test");
            var info2 = prc.Single.Get("GetClient_Test");

            Assert.AreEqual(info2.ToString(), "GetClient_Test");

            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Pool_Single_For()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 10
            });
            for (int i = 0; i < 100; i++)
            {
                prc.Single.Set("GetMaxClient_Test" + i, "GetMaxClient_Test");

                var info2 = prc.Single.Get("GetMaxClient_Test" + i);

                Assert.AreEqual(info2.ToString(), "GetMaxClient_Test");
            }
           prc.Dispose();
        }
       
        [TestMethod, TestCategory("poolRedisclient")]
        public void Pool_Single_Parallel()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 20,
                MinClients = 10,
            });
            Parallel.For(0, 1000, new ParallelOptions() {MaxDegreeOfParallelism = 100}, (index, item) =>
            {
               
                prc.Single.Set("Parallel_PoolClient_Test" + index, "Parallel_PoolClient_Test");

                var info2 = prc.Single.Get("Parallel_PoolClient_Test" + index);

                Assert.AreEqual(info2.ToString(), "Parallel_PoolClient_Test");
            });
            Thread.Sleep(5000);
            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Pool_Send_Get_TimeOut()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 2,
                MinClients = 1,
                SendTimeout = 5,
                ReceiveTimeout = 5
            });
            var par = Parallel.For(0, 1000, new ParallelOptions() {MaxDegreeOfParallelism = 100}, (index, item) =>
            {
                Thread.Sleep(100);
                prc.Single.Set("Parallel_PoolClient_Test" + index, "Parallel_PoolClient_Test");

                var info2 = prc.Single.Get("Parallel_PoolClient_Test" + index);

                Assert.AreEqual(info2.ToString(), "Parallel_PoolClient_Test");
            });
            Thread.Sleep(5000);
            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Pool_Single_TimeOut()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port,
                MaxClients = 10
            });
            prc.Single.Set("PoolClient_TimeOut_Test", "PoolClient_TimeOut_Test");
            Thread.Sleep(15000);
            object info2 = prc.Single.Get("PoolClient_TimeOut_Test");

            Assert.AreEqual(info2.ToString(), "PoolClient_TimeOut_Test");

            prc.Dispose();
        }

        [TestMethod, TestCategory("poolRedisclient")]
        public void Pool_Single_Parallel_MultiThread()
        {
            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration()
            {
                Host = ip,
                Port = port
            });
            Parallel.For(0, 1000, new ParallelOptions() {MaxDegreeOfParallelism = 100}, (index, item) =>
            {
                var t = new Thread(() =>
                {
                    Thread.Sleep(100);

                    prc.Single.Set("Parallel_PoolClient_Test" + index, "Parallel_PoolClient_Test");

                    object info2 = prc.Single.Get("Parallel_PoolClient_Test" + index);

                    Assert.AreEqual(info2.ToString(), "Parallel_PoolClient_Test");
                });
                t.Start();
            });
        }
    }
}
