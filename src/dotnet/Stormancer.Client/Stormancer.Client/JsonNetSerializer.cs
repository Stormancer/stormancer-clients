using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    internal class JsonNetSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json)!;
        }

        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
