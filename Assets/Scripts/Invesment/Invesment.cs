using System;
using Newtonsoft.Json;

namespace Invesment
{
    [Serializable]
    public class Invesment
    {
        [JsonIgnore]
        public Data Data => Types.Dictionary[type].subTypes[sub_type];

        public ulong id;
      
        public string type;
        public string sub_type;

        public short age;
        public string name;

        public ushort shares;
        public uint baseValue;

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

