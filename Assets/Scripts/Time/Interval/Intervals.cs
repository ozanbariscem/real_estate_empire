using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Time
{
    [Serializable]
    [MoonSharpUserData]
    public class Intervals
    {
        public Interval[] interval = new Interval[5];
        public Interval Interval => interval[selectedInterval];


        [NonSerialized] private int selectedInterval = 0;
        public  int SelectedInterval => selectedInterval;

        public bool SelectInterval(int select)
        {
            if (select < 0) return false;

            if (select < interval.Length)
            {
                selectedInterval = select;
                return true;
            }
            return false;
        }

        public static Intervals FromJson(string json)
        {
            return JsonUtility.FromJson<Intervals>(json);
        }

        public static string ToJson(Intervals intervals)
        {
            return JsonUtility.ToJson(intervals);
        }
    }
}

