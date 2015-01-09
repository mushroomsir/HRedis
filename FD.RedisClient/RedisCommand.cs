
namespace FD.RedisClient
{
    public enum RedisCommand
    {
        AUTH, //服务器密码验证
        DEL, //删除
        GET, //获取一个key的值
        INFO, //Redis信息。  
        SET,  //添加一个值
    }
}
