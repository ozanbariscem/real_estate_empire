using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Investment
{
    [Serializable][MoonSharpUserData]
    public class Data
    {
        public Dictionary<string, string> traits = new Dictionary<string, string>()
        {
            { "ageable", "true" },
            { "buyable", "true" },
            { "physical", "true" },
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

