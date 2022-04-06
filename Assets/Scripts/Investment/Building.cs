using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Investment.Property
{
    [MoonSharpUserData]
    public class Building
    {
        public int id;

        public string name;
        public string district_tag;

        public int age;
        public int floor_count;
        public int floor_size;

        public List<int> apartments;
    }
}
