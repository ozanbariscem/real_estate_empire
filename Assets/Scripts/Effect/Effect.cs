using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Effect
{
    [MoonSharpUserData]
    public class Effect
    {
        public enum Tags { value }
        public enum Types { percentage, value }

        public static Dictionary<Tags, Effect> Effects { get; private set; }

        public Tags tag;
        public Types type;

        public static Dictionary<Tags, Effect> LoadEffects(List<Effect> effects)
        {
            if (Effects == null)
                Effects = new Dictionary<Tags, Effect>();

            foreach (Effect effect in effects)
            {
                if (Effects.TryGetValue(effect.tag, out Effect oldEffect))
                {
                    oldEffect = effect;
                }
                else
                {
                    Effects.Add(effect.tag, effect);
                }
            }

            return Effects;
        }
    }
}

