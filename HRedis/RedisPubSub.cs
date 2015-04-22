using System;
using System.Net.Sockets;
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

        private int Status = 1;
        private string ChannelName = string.Empty;
        private int RetryCount = 0;

        private Thread thread;

        public RedisPubSub(RedisConfiguration config)
        {
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
            do
            {
                if (OnMessage != null)
                    OnMessage(this, client.ReadData());
                else
                    client.ReadData();

            } while (Status == 1);
        }

        public object Publish(string channel, string message)
        {
            return client.Send(RedisCommand.PUBLISH, channel, message);
        }

        public void Subscribe(string channelName)
        {
            Init(channelName);
            Subscribe();
        }

        public void PSubscribe(string channelName)
        {
            Init(channelName);
            Subscribe(isPattern: true);
        }


        public void UnSubscribe(string channelName)
        {
            UnSubscribe();
        }

        public void UnPSubscribe(string channelName)
        {
            UnSubscribe(isPattern: true);
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

        private void Subscribe(bool isPattern = false)
        {
            thread = new Thread(n => CheckSubscribe((bool) n));
            thread.Start(isPattern);
        }

        private void CheckSubscribe(bool isPattern = false)
        {
            while (RetryCount <= 3 && Status == 1)
            {
                try
                {
                    if (RetryCount > 0)
                        Thread.Sleep(RetryCount*1000);

                    Interlocked.Exchange(ref Status, 1);
                    Run(isPattern);
                }
                catch (SocketException exception)
                {
                    if (OnError != null)
                        OnError(exception);
                    break;
                }
                catch (ThreadAbortException exception) { }
                catch (Exception exception)
                {
                    Interlocked.Exchange(ref Status, 0);
                    Interlocked.Increment(ref RetryCount);
                    if (RetryCount == 4)
                    {
                        if (OnError != null)
                            OnError(exception);
                        break;
                    }
                }
            }
        }

        private void UnSubscribe(bool isPattern = false)
        {
            Interlocked.Exchange(ref Status, 0);
            object repy;
            if (isPattern)
                repy = client.Send(RedisCommand.PUNSUBSCRIBE, ChannelName);
            else
                repy = client.Send(RedisCommand.UNSUBSCRIBE, ChannelName);


            if (OnUnSubscribe != null)
                OnUnSubscribe(repy);

        }

        public void Dispose()
        {
            Interlocked.Exchange(ref Status, 0);
             if (thread != null)
                thread.Abort();
            client.Dispose();
        }
    }
}
