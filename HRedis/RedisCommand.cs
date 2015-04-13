/*
 * Document http://redis.io/commands
 */
namespace HRedis
{
    public enum RedisCommand
    {
        GET,
        INFO,
        SET,
        EXPIRE,
        MULTI,
        EXEC,
        QUIT,
        SUBSCRIBE,
        UNSUBSCRIBE,
        PSUBSCRIBE,
        PUNSUBSCRIBE,
        PUBLISH,
        PUBSUB
    }
}
