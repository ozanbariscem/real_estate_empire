using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Time
{
    [Serializable]
    [MoonSharpUserData]
    public class Date
    {
        public ushort hour;
        public ushort day;
        public ushort month;
        public ushort year;

        public string ToNumberString()
        {
            return $"{hour}:00/{day}.{month}.{year}";
        }

        /// <summary>
        /// Passes time according to given calendar
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns>Represents the pass states of hour ([0]), day ([1]), month ([2) and year ([3])</returns>
        public bool[] PassTime(Calendar.Calendar calendar)
        {
            bool[] hasPassed = new bool[4];

            hasPassed[0] = true;
            if (hour + 1 < calendar.hours)
                hour++;
            else
            {
                hour = 0;
                hasPassed[1] = true;
                if (day + 1 <= calendar.months[month - 1].days)
                    day++;
                else
                {
                    day = 1;
                    hasPassed[2] = true;
                    if (month + 1 <= calendar.months.Length)
                        month++;
                    else
                    {
                        month = 1;
                        year++;
                        hasPassed[3] = true;
                    }
                }
            }

            return hasPassed;
        }

        public static Date FromJson(string json)
        {
            return JsonUtility.FromJson<Date>(json);
        }

        public static string ToJson(Date obj)
        {
            return JsonUtility.ToJson(obj);
        }
    }
}
