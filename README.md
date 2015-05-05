
##配置说明：

            new RedisClient(new RedisConfiguration()
            {
                Host = "127.0.0.1",     //服务器
                Port = 6379,            //端口
                PassWord = "123456",    //redis密码，没有则不填
                ReceiveTimeout = 0,     //接收超时时间
                SendTimeout = 0         //发送超时时间
            });

##基本连接


            using (RedisClient client = new RedisClient("127.0.0.1", 6381))
            {
                client.Set("key", "value");
                client.Get("key");
            }



##使用连接池


            PoolRedisClient prc = new PoolRedisClient(new PoolConfiguration());
    
            prc.Single.Set("key", "value");

            prc.Single.Get("key");

    
       
           


