using System.Collections.Generic;

namespace Loan
{
    public class Types
    {
        private static Dictionary<int, Type> types;
        public static Dictionary<int, Type> Dictionary => types;

        public static Type GetType(int id)
        {
            if (types != null && types.ContainsKey(id)) return types[id];
            return null;
        }

        public static void SetTypes(List<Type> newTypes)
        {
            if (types == null)
                types = new Dictionary<int, Type>();

            foreach (Type type in newTypes)
            {
                // override
                if (types.ContainsKey(type.id))
                {
                    types[type.id] = type;
                } else // add new
                {
                    types.Add(type.id, type);
                }
            }
        }
    }
}

