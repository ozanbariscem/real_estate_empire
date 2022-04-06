using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace District
{
    [MoonSharpUserData]
    public class Data
    {
        public static Dictionary<string, Data> Datas;

        public string tag;
        public string name;
        public int size;

        public List<int> buildings;

        public static Data SafeGetData(string tag)
        {
            Datas.TryGetValue(tag, out Data data);
            return data;
        }

        public static Dictionary<string, Data> LoadJson(string json)
        {
            List<Data> dataList = JsonConvert.DeserializeObject<List<Data>>(json);

            if (Datas == null)
                Datas = new Dictionary<string, Data>();

            foreach (Data data in dataList)
            {
                if (Datas.TryGetValue(data.tag, out Data oldData))
                    oldData = data;
                else
                    Datas.Add(data.tag, data);
            }
            return Datas;
        }
    }
}
