using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace RandomEvent
{
    [MoonSharpUserData]
    public class EventDictionary
    {
        public static Dictionary<string, Script> Dictionary { get; private set; }

        public static void LoadEvents(Dictionary<string, Script> events)
        {
            if (Dictionary == null)
                Dictionary = new Dictionary<string, Script>();

            foreach (var key in events.Keys)
            { 
                if (Dictionary.TryGetValue(key, out Script script))
                {
                    script = events[key];
                } else
                {
                    Dictionary.Add(key, events[key]);
                }
            }
        }
    }
}
