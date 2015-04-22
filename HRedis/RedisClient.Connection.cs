using System;

namespace HRedis
{
    public partial class RedisClient
    {
        public bool Ping()
        {
            return Execute(RedisCommand.PING).ToString().Equals(ReplyFormat.PingSuccess);
        }
        public bool Select()
        {
            return Execute(RedisCommand.SELECT).ToString().Equals(ReplyFormat.ReplySuccess);
        }
    }
}
