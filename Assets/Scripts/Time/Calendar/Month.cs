using System;
using MoonSharp.Interpreter;

namespace Calendar
{
    [Serializable]
    [MoonSharpUserData]
    public class Month
    {
        public ushort days;
        public string name;
    }
}
