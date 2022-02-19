using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace District
{
    [MoonSharpUserData]
    public class District
    {
        [JsonIgnore] public Data Data => Data.SafeGetData(tag);
        [JsonIgnore] public string Name => Data.name;
        [JsonIgnore] public int Size => Data.size;

        public string tag;
        public int population;

        public List<int> properties;
    }
}
