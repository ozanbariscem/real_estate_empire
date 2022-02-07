using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Ownership
{
    [MoonSharpUserData]
    public class OwnershipList
    {
        private static Dictionary<string, Dictionary<int, List<Ownership>>> categorizedToInvesments;
        private static List<Ownership> list;
        public static List<Ownership> List => list;

        public OwnershipList() { }

        public OwnershipList(List<Ownership> list)
        {
            OwnershipList.list = list;
            categorizedToInvesments = CategorizeToInvesments(list);
        }
    
        private Dictionary<string, Dictionary<int, List<Ownership>>> CategorizeToInvesments(List<Ownership> list)
        {
            Dictionary<string, Dictionary<int, List<Ownership>>> categorized = new Dictionary<string, Dictionary<int, List<Ownership>>>();
            
            foreach (Ownership ownership in list)
            {
                if (!categorized.ContainsKey(ownership.invesment_type))
                {
                    categorized.Add(ownership.invesment_type, new Dictionary<int, List<Ownership>>());
                }
                if (!categorized[ownership.invesment_type].ContainsKey(ownership.invesment_id))
                {
                    categorized[ownership.invesment_type].Add(ownership.invesment_id, new List<Ownership>());
                }
                categorized[ownership.invesment_type][ownership.invesment_id].Add(ownership);
            }

            return categorized;
        }

        public static Ownership GetOwnership(int id)
        {
            if (id < list.Count)
            {
                if (list[id].id == id)
                    return list[id];
                else
                    Console.Console.Run("log_error id mismatch at OwnershipList, make sure index of members represents their id!");
            }
            return null;
        }

        public static List<Ownership> GetOwnershipsOfInvesment(string type, int id)
        {
            if (categorizedToInvesments.ContainsKey(type))
            {
                if (categorizedToInvesments[type].ContainsKey(id))
                {
                    return categorizedToInvesments[type][id];
                }
            }
            return null;
        }
    }
}
