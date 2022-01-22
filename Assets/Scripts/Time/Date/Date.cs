using System;
using UnityEngine;

namespace Time
{
    [Serializable]
    public class Date
    {
        public ushort day;
        public ushort month;
        public ushort year;

        public string ToNumberString()
        {
            return $"{day}.{month}.{year}";
        }

        /// <summary>
        /// Passes day according to given calendar
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns>Represents the pass states of day ([0]), month ([1]) and year ([2])</returns>
        public bool[] PassDay(Calendar.Calendar calendar)
        {
            bool[] hasPassed = new bool[3];

            hasPassed[0] = true;
            if (day + 1 <= calendar.months[month-1].days)
                day++;
            else
            {
                day = 1;
                hasPassed[1] = true;
                if (month + 1 <= calendar.months.Length)
                    month++;
                else
                {
                    month = 1;
                    year++;
                    hasPassed[2] = true;
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
