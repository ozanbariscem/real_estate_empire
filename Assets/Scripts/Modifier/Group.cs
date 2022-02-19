using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Modifier
{
    [MoonSharpUserData]
    public class Group
    {
        public static Dictionary<string, Group> Groups;

        public string tag;

        public static Dictionary<string, Group> LoadGroups(List<Group> groups)
        {
            if (Groups == null)
                Groups = new Dictionary<string, Group>();

            foreach (Group group in groups)
            {
                if (Groups.TryGetValue(group.tag, out Group oldGroup))
                {
                    oldGroup = group;
                } else
                {
                    Groups.Add(group.tag, group);
                }
            }

            return Groups;
        }
    }
}

