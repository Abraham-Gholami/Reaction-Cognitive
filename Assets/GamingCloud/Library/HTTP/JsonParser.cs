using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gamingCloud
{

    public class JsonParser
    {
        public static T ConvertDictionaryToSchema<T>(Dictionary<string, dynamic> json)
        {
            string stringJson = JsonConvert.SerializeObject(json);
            return JObject.Parse(stringJson).ToObject<T>();
        }
        public static T ConvertDictionaryToSchema<T>(Dictionary<string, string> json)
        {
            string stringJson = JsonConvert.SerializeObject(json);
            return JObject.Parse(stringJson).ToObject<T>();
        }
    }
}
