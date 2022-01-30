using System.Collections.Generic;

namespace Ownership
{
    public class OwnershipList
    {
        private static List<Ownership> list;

        public OwnershipList(List<Ownership> list)
        {
            OwnershipList.list = list;
        }
    
        public static Ownership SafeGetOwnership(int id)
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
    }
}
