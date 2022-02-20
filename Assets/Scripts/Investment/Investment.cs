using System;
using System.Collections.Generic;
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
        public Type Type => Types.Dictionary[type];

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

        [JsonIgnore]
        public uint value;

        public uint CalculateValue()
        {
            float modifiers_effect = ModifiersEffectToValue();

            Script script = Type.script;
            if (script.Globals["CalculateValue"] != null)
            {
                return (uint)Type.script.Call(
                    script.Globals["CalculateValue"],
                    id,
                    baseValue,
                    modifiers_effect
                    ).Number;
            }
            else
            {
                // Console.Console.Run($"log Hello my id is {id} and i'm worth {(uint)(baseValue * modifiers_effect)} but my base value was {baseValue}");
                return (uint)(baseValue * modifiers_effect);
            }
        }

        private float ModifiersEffectToValue()
        {
            Dictionary<string, Modifier.Modifier>.ValueCollection modifiers
                = Modifier.ModifierDictionary.Modifiers[type][id].Values;

            uint dummy = baseValue;
            foreach (Modifier.Modifier modifier in modifiers)
            {
                foreach (var effectData in modifier.Data.effects)
                {
                    Effect.Effect effect = Effect.Effect.Effects[Effect.Effect.Tags.value];

                    if (effect.tag == Effect.Effect.Tags.value)
                    {
                        if (effect.type == Effect.Effect.Types.percentage)
                        {
                            dummy = (uint)(dummy * ((effectData.amount + 100) / 100f));
                        }
                        if (effect.type == Effect.Effect.Types.value)
                        {
                            dummy = (uint)(dummy + ((effectData.amount + 100) / 100f));
                        }
                    }
                }
            }
            return (float)dummy / (float)baseValue;
        }

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

