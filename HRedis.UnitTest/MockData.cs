using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRedis.UnitTest
{
    public  class MockData
    {
        public static readonly string MasterIp = "127.0.0.1";
        public static readonly int MasterPort = 6379;

        public static readonly string SlaveIp = "127.0.0.1";
        public static readonly int SlavePort = 6381;


        public static readonly string SentinelIp = "127.0.0.1";
        public static readonly int SentinelPort = 200001;


    }
}
