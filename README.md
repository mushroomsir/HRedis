

##基本连接
需要手动释放。

            using (RedisClient client = new RedisClient("127.0.0.1", 6381))
            {
                Console.WriteLine(client.Set("key", "value"));
                Console.WriteLine(client.Get("key"));
            }

配置说明：

            new RedisClient(new RedisConfiguration()
            {
                Host = "127.0.0.1",     //服务器
                Port = 6379,            //端口
                PassWord = "123456",    //redis密码，没有则不填
                ReceiveTimeout = 0,     //接收超时时间
                SendTimeout = 0         //发送超时时间
            });


##使用池命令
命令使用结束后，会自动释放连接。

            PoolRedisClient prc = new PoolRedisClient("127.0.0.1", 6381);
            prc.Cmd.Info();


##使用池获取连接
需要手动释放。

            PoolRedisClient prc = new PoolRedisClient("127.0.0.1", 6381);
            using (var client = prc.GetClient())
            {
                Console.WriteLine(client.Get("key"));
            }
