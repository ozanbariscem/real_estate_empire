using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Invesment
{
    public static class Types
    {
        private static Dictionary<string, Type> types;
        public static Dictionary<string, Type> Dictionary => types;

        public static void LoadJson(string json)
        {
            types = JsonConvert.DeserializeObject<Dictionary<string, Type>>(json);
        }
    }
}

