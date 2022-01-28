using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Invesment
{
    [Serializable]
    public class Data
    {
        public Dictionary<string, string> traits = new Dictionary<string, string>()
        {
            { "ageable", "true" },
            { "buyable", "true" },
            { "partially_ownable", "true" },
        };

        public bool Is(string trait)
        {
            return traits.ContainsKey(trait) && traits[trait] == "true";
        }

        public static string ToJson(Data data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static Data FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Data>(json);
        }
    }
}

