/*
 * Document http://redis.io/commands
 */
namespace FD.RedisClient
{
    
    public enum RedisCommand
    {
        GET, //获取一个key的值
        INFO, //Redis信息。  
        SET, //添加一个值
        EXPIRE, //设置过期时间
        MULTI, //标记一个事务块开始
        EXEC, //执行所有 MULTI 之后发的命令
        QUIT,//Ask the server to close the connection.
    }
}
