using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Job
{
    [MoonSharpUserData]
    public class Level
    {
        public float popularity;
        public uint salary;

        public List<Effect> effects;
    }
}
