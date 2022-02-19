using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Modifier
{
    [MoonSharpUserData]
    public class ModifierDictionary
    {
        public static Dictionary<string, 
            Dictionary<string, Dictionary<string, bool>>> ActiveModifiers { get; private set; }

        public static Dictionary<string, Modifier> Modifiers { get;  private set; }

        public static Dictionary<string, Modifier> LoadModifiers(List<Modifier> modifiers)
        {
            if (Modifiers == null)
                Modifiers = new Dictionary<string, Modifier>();

            foreach (Modifier modifier in modifiers)
            {
                if (Modifiers.TryGetValue(modifier.tag, out Modifier oldModifier))
                {
                    oldModifier = modifier;
                }
                else
                {
                    Modifiers.Add(modifier.tag, modifier);
                }
            }

            return Modifiers;
        }
    
        public static Dictionary<string,
            Dictionary<string, Dictionary<string, bool>>> LoadActiveModifiers(string group, Dictionary<string, Dictionary<string, bool>> modifiers)
        {
            if (ActiveModifiers == null)
                ActiveModifiers = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>();

            if (!ActiveModifiers.ContainsKey(group))
                ActiveModifiers.Add(group, new Dictionary<string, Dictionary<string, bool>>());

            foreach (var key in modifiers.Keys)
            {
                if (ActiveModifiers[group].ContainsKey(key))
                {
                    foreach (var modifier_tag in modifiers[key].Keys)
                    {
                        if (ActiveModifiers[group][key].ContainsKey(modifier_tag))
                        {
                            ActiveModifiers[group][key][modifier_tag] = modifiers[key][modifier_tag];
                        } else
                        {
                            ActiveModifiers[group][key].Add(modifier_tag, modifiers[key][modifier_tag]);
                        }
                    }
                } else
                {
                    ActiveModifiers[group].Add(key, modifiers[key]);
                }
            }

            return ActiveModifiers;
        }
    }
}
