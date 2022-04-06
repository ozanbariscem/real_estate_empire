using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Investment
{
    [MoonSharpUserData]
    public abstract class Investment
    {
        [JsonProperty(Order = -2)]
        public int id; 
        [JsonProperty(Order = -2)]
        public Type type;
    }
}
