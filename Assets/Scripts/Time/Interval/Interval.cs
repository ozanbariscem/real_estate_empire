using System;
using MoonSharp.Interpreter;

namespace Time
{
    [Serializable]
    [MoonSharpUserData]
    public class Interval
    {
        public float tick_in_seconds = 1f;
    }
}

