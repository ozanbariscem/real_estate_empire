using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Investment.Property
{
    [MoonSharpUserData]
    public class Apartment : Investment
    {
        public int building_id;
        public int floor_index;
        public int door_index;


        // Dummy to be changed
        public int price;

        public Apartment()
        {
            type = Type.Apartment;
        }
    }
}
