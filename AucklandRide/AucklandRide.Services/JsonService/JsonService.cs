﻿using Newtonsoft.Json;

namespace Services.JsonService
{
    public class JsonService : IJsonService
    {
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

    }
}
