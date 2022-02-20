using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Modifier
{
    [MoonSharpUserData]
    public class ModifierData
    {
        public string tag;
        public string name;
        public string description;
        public Time.Date effective;

        public string icon;

        public List<EffectData> effects;
    }
}
