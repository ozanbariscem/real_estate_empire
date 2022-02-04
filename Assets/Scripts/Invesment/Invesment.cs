using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Invesment
{
    [Serializable]
    public class Invesment
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

        public static Invesment FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Invesment>(json);
        }

        public static string ToJson(Invesment invesment)
        {
            return JsonConvert.SerializeObject(invesment);
        }
    }
}

