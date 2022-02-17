using System;
using MoonSharp.Interpreter;
using Newtonsoft.Json;

namespace Loan
{
    [Serializable]
    [MoonSharpUserData]
    public class Loan
    {
        [JsonIgnore]
        public Type Type => Types.GetType(type_id);

        public int id;
        public int type_id;
        public int investor_id;

        public uint amountLeft;
        public byte paymentLeft;
    }
}
