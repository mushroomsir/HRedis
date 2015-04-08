/*
 * Document http://redis.io/commands
 */
namespace HRedis
{
    public enum RedisCommand
    {
        /// <summary>
        /// 获取一个key的值
        /// </summary>
        GET,

        /// <summary>
        /// Redis信息  
        /// </summary>
        INFO,

        /// <summary>
        /// 添加一个值
        /// </summary>
        SET,

        /// <summary>
        /// 设置过期时间
        /// </summary>
        EXPIRE,

        /// <summary>
        ///标记一个事务块开始
        /// </summary>
        MULTI,

        /// <summary>
        /// 执行所有 MULTI 之后发的命令
        /// </summary>
        EXEC,

        /// <summary>
        /// 通知服务器关闭当前连接
        /// </summary>
        QUIT,

        /// <summary>
        /// 订阅普通渠道
        /// </summary>
        SUBSCRIBE,

        /// <summary>
        /// 订阅模式匹配渠道
        /// </summary>
        PSUBSCRIBE,
    }
}
