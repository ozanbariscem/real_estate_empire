using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effect
{
    public class Effect
    {
        public static Dictionary<string, Effect> Effects { get; private set; }

        public string tag;
        public string type;

        public static Dictionary<string, Effect> LoadEffects(List<Effect> effects)
        {
            if (Effects == null)
                Effects = new Dictionary<string, Effect>();

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

