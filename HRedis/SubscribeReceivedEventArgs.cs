using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRedis
{
    public class SubscribeReceivedEventArgs : EventArgs
    {
        public string Body { get; set; }
    }
}
