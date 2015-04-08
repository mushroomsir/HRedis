
namespace HRedis
{
    public class MessageFormat
    {
        /// <summary>
        /// *number of arguments\r\n
        /// </summary>
        public static readonly string Head = "*{0}\r\n";

        /// <summary>
        /// $number of bytes of argument N\r\nargument data\r\n
        /// </summary>
        public static readonly string Argument = "${0}\r\n{1}\r\n";

        public static readonly char CR = '\r';

        public static readonly char LF = '\n';
    }

    public enum RedisMessage
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        Error = '-',

        /// <summary>
        /// 状态消息 +
        /// </summary>
        Status = '+',

        /// <summary>
        /// 大块消息 $
        /// </summary>
        Bulk = '$',

        /// <summary>
        /// 多条大块消息 *
        /// </summary>
        MultiBulk = '*',

        /// <summary>
        /// 数字消息:
        /// </summary>
        Int = ':',
    }
}
