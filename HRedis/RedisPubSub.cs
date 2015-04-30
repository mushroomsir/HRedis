using System;
using System.Diagnostics;
using System.Threading;

namespace HRedis
{
    public sealed class RedisPubSub : IDisposable
    {

        private RedisBaseClient client;
        public Action<object, object> OnMessage;
        public Action<object[]> OnSuccess;
        public Action<object> OnUnSubscribe;
        public Action<Exception> OnError;

        private string ChannelName = string.Empty;
        private bool IsPattern = false;

        private Thread thread;

        public RedisConfiguration Configuration;

        public RedisPubSub(RedisConfiguration config)
        {
            Configuration = config;
            client = new RedisBaseClient(config);
        }

        public RedisPubSub(string ip, int port)
            : this(new RedisConfiguration()
            {
                Host = ip,
                Port = port,
            })
        {

        }

        private void Listen()
        {
            while (true)
            {
                var reply = client.ReadData();
                if (reply is object[])
                {
                    var val = reply as object[];
                    if (val[0].ToString().ToLower().Contains("unsubscribe"))
                    {
                        if (OnUnSubscribe != null)
                            OnUnSubscribe(val);
                        break;
                    }
                }
                if (OnMessage != null)
                    OnMessage(this, reply);
            }
        }

        public object Publish(string channel, string message)
        {
            return client.Send(RedisCommand.PUBLISH, channel, message);
        }

        public void Subscribe(string channelName)
        {
            Init(channelName);
            thread = new Thread(() => CheckSubscribe(IsPattern));
            thread.Start();
        }

        public void PSubscribe(string channelName)
        {
            Init(channelName);
            IsPattern = true;
            thread = new Thread(() => CheckSubscribe(IsPattern));
            thread.Start();
        }

        public void UnSubscribe(string channelName)
        {
            UnSubscribe();
        }

        public void UnPSubscribe(string channelName)
        {
            UnSubscribe();
        }

        private void Init(string channelName)
        {
            ChannelName = channelName;
        }

        private void Run(bool isPattern = false)
        {
            object repy;
            if (isPattern)
                repy = client.Send(RedisCommand.PSUBSCRIBE, ChannelName);
            else
                repy = client.Send(RedisCommand.SUBSCRIBE, ChannelName);

            if (OnSuccess != null)
                OnSuccess(repy as object[]);

            Listen();
        }

        private void CheckSubscribe(bool isPattern = false)
        {

            try
            {
                Run(isPattern);
            }
            catch (ThreadAbortException exception)
            {
            }
            catch (Exception exception)
            {
                if (OnError != null)
                    OnError(exception);
            }
        }

        private void UnSubscribe()
        {
            try
            {
                if (IsPattern)
                    client.SendN(RedisCommand.PUNSUBSCRIBE.ToString(), ChannelName);
                else
                    client.SendN(RedisCommand.UNSUBSCRIBE.ToString(), ChannelName);
            }
            catch (Exception exception)
            {
                Debug.Print("Hredis UnSubscribe:" + exception.Message);
            }
        }

        public void Dispose()
        {
            if (thread != null)
                thread.Abort();
            
            client.Dispose();
        }
    }
}
