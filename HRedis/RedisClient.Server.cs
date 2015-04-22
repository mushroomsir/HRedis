using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRedis
{
    public partial class RedisClient
    {

        public Dictionary<string, string> Info
        {
            get
            {
                var info = Execute(RedisCommand.INFO).ToString();

                var dc = new Dictionary<string, string>();

                foreach (var line in info
                    .Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var p = line.IndexOf(':');
                    if (p == -1) continue;

                    dc.Add(line.Substring(0, p), line.Substring(p + 1));
                }
                return dc;
            }
        }
    }
}
