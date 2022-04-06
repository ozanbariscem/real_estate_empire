using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Modifier
{
    [MoonSharpUserData]
    public class ModifierDictionary
    {
        // Modifiers dictionary holds 
        // Investment.type, Investment.id, Modifier.tag, Modifier

        // i.e
        // Modifiers["property"][0] returns the Dictionary of modifiers property 0 has
        // Modifiers["property"][0]["homeless"] returns the property 0's "homeless" modifier
        // OR returns null if property 0 doesn't have any "homeless" modifier

        public static Dictionary<string,
            Dictionary<int, Dictionary<string, Modifier>>> Modifiers { get; private set; }

        public static List<Modifier> ModifiersSorted { get; private set; }

        public static Dictionary<string, ModifierData> Dictionary { get;  private set; }

        public static void LoadModifierDatas(List<ModifierData> modifiers)
        {
            if (Dictionary == null)
            {
                Dictionary = new Dictionary<string, ModifierData>();
                Modifiers = new Dictionary<string, Dictionary<int, Dictionary<string, Modifier>>>();
            }

            foreach (ModifierData modifier in modifiers)
            {
                if (Dictionary.TryGetValue(modifier.tag, out ModifierData oldModifier))
                {
                    oldModifier = modifier;
                }
                else
                {
                    Dictionary.Add(modifier.tag, modifier);
                }
            }
        }

        public static void LoadModifiers(Dictionary<string, Dictionary<int, Dictionary<string, Modifier>>> modifiers)
        {
            Modifiers = modifiers;
        }

        public static void AddModifier(string type, int id, Modifier modifier)
        {
            if (Modifiers == null)
                Modifiers = new Dictionary<string, Dictionary<int, Dictionary<string, Modifier>>>();

            if (Modifiers.TryGetValue(type, out Dictionary<int, Dictionary<string, Modifier>> types))
            {
                if (types.TryGetValue(id, out Dictionary<string, Modifier> ids))
                {
                    if (ids.TryGetValue(modifier.Data.tag, out Modifier _modifier))
                    {
                        _modifier = modifier;
                    } else
                    {
                        ids.Add(modifier.Data.tag, modifier);
                    }
                } else
                {
                    types.Add(id, new Dictionary<string, Modifier>());
                    types[id].Add(modifier.Data.tag, modifier);
                }
            } else
            {
                Modifiers.Add(type, new Dictionary<int, Dictionary<string, Modifier>>());
                Modifiers[type].Add(id, new Dictionary<string, Modifier>());
                Modifiers[type][id].Add(modifier.Data.tag, modifier);
            }

            AddSorted(modifier);
        }

        public static Modifier RemoveModifier(string type, int id, string modifier_tag)
        {
            if (Modifiers.TryGetValue(type, out Dictionary<int, Dictionary<string, Modifier>> types))
            {
                if (types.TryGetValue(id, out Dictionary<string, Modifier> modifiers))
                {
                    if (modifiers.TryGetValue(modifier_tag, out Modifier modifier))
                    {
                        modifiers.Remove(modifier_tag);
                        return modifier;
                    }
                }
            }
            return null;
        }
    
        public static void AddSorted(Modifier item)
        {
            if (ModifiersSorted == null)
            {
                ModifiersSorted = new List<Modifier>() { item };
                return;
            }
            if (ModifiersSorted.Count == 0)
            {
                ModifiersSorted.Add(item);
                return;
            }
            if (ModifiersSorted[ModifiersSorted.Count - 1].CompareTo(item) <= 0)
            {
                ModifiersSorted.Add(item);
                return;
            }
            if (ModifiersSorted[0].CompareTo(item) >= 0)
            {
                ModifiersSorted.Insert(0, item);
                return;
            }
            int index = ModifiersSorted.BinarySearch(item);
            if (index < 0)
                index = ~index;
            ModifiersSorted.Insert(index, item);
        }
    }
}
