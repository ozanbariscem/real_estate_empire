using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Person
{
    [MoonSharpUserData]
    public abstract class Person
    {
        public string name;
        public string surname;
    }
}
