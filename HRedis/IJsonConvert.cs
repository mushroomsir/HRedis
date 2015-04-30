using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRedis
{
    public interface IJsonConvert
    {
        string SerializeObject(object value);
        T DeserializeObject<T>(string value);
    }

    public class JsonConvert : IJsonConvert
    {
        public string SerializeObject(object value)
        {
           return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
        public T DeserializeObject<T>(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }
    }
}
