using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System;
using Time;

namespace Modifier
{
    [MoonSharpUserData]
    public class Modifier : IComparable<Modifier>
    {
        [JsonIgnore] public string investment_type;
        [JsonIgnore] public int investment_id;

        public string modifier_data_tag;
        
        private ModifierData data;
        [JsonIgnore] public ModifierData Data => data;

        public Date endDate;

        public Modifier(string investment_type, int investment_id, string modifier_data_tag, Date endDate)
        {
            this.investment_type = investment_type;
            this.investment_id = investment_id;
            this.modifier_data_tag = modifier_data_tag;
            this.endDate = endDate;

            data = ModifierDictionary.Dictionary[modifier_data_tag];
        }

        public int CompareTo(Modifier other)
        {
            return endDate.CompareTo(other.endDate);
        }
    }
}

