using System;
using UnityEngine;

namespace Calendar
{
    [Serializable]
    public class Calendar
    {
        public Month[] months = new Month[12];
    
        public static Calendar FromJson(string json)
        {
            return JsonUtility.FromJson<Calendar>(json);
        }

        public static string ToJson(Calendar calendar)
        {
            return JsonUtility.ToJson(calendar);
        }
    }
}
