using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Investment
{
    [Serializable]
    [MoonSharpUserData]
    public class Investment
    {
        [JsonIgnore]
        public Data Data => Types.Dictionary[type].subTypes[sub_type];

        public int id;
      
        public string type;
        public string sub_type;

        [JsonProperty("photo")]
        public string photo;

        [JsonIgnore]
        public Texture2D texture;

        public string name;
        public short age;

        public ushort shares;
        public uint baseValue;

        public override string ToString()
        {
            return $"{type}-{sub_type} {name} {age} {shares} {baseValue}";
        }

        public static Investment FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Investment>(json);
        }

        public static string ToJson(Investment invesment)
        {
            return JsonConvert.SerializeObject(invesment);
        }
    }
}

