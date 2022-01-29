using System.Collections.Generic;
using UnityEngine;

namespace Invesment
{
    public class InvesmentDictionary
    {
        private static Dictionary<string, List<Invesment>> invesments;

        public InvesmentDictionary(Dictionary<string, List<Invesment>> dictionary)
        {
            invesments = dictionary;
        }

        public static Invesment SafeGetInvesment(string tag, int id)
        {
            if (invesments.ContainsKey(tag) && id < invesments[tag].Count) return invesments[tag][id];
            return null;
        }
    }
}
