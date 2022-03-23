using System.Collections.Generic;
using MoonSharp.Interpreter;
using System.Linq;
using System;

namespace Ownership
{
    [MoonSharpUserData]
    public class OwnershipDictionary
    {
        public static event EventHandler<Ownership> OnOwnershipAdded;

        public static Dictionary<string, Dictionary<string, Dictionary<int, Ownership>>>
            Dictionary { get; private set; }

        public static void AddOwnerships(List<Ownership> ownerships)
        {
            if (Dictionary == null)
                Dictionary = new Dictionary<string, Dictionary<string, Dictionary<int, Ownership>>>();

            foreach (var ownership in ownerships)
            {
                if (Dictionary.TryGetValue(ownership.company_tag, out var investments))
                {
                    if (investments.TryGetValue(ownership.investment_type, out var investment_types))
                    {
                        if (investment_types.TryGetValue(ownership.investment_id, out var registered_ownership))
                        {
                            registered_ownership = ownership;
                            // OnOwnershipChanged?.Invoke(null, registered_ownership);
                        } else
                        {
                            investment_types.Add(ownership.investment_id, ownership);
                            OnOwnershipAdded?.Invoke(null, ownership);
                        }
                    } else
                    {
                        investments.Add(ownership.investment_type, new Dictionary<int, Ownership>());
                        investments[ownership.investment_type].Add(ownership.investment_id, ownership);
                        OnOwnershipAdded?.Invoke(null, ownership);
                    }
                } else
                {
                    Dictionary.Add(ownership.company_tag, new Dictionary<string, Dictionary<int, Ownership>>());
                    Dictionary[ownership.company_tag].Add(ownership.investment_type, new Dictionary<int, Ownership>());
                    Dictionary[ownership.company_tag][ownership.investment_type].Add(ownership.investment_id, ownership);
                    OnOwnershipAdded?.Invoke(null, ownership);
                }
            }
        }
    
        public static List<Ownership> ToList()
        {
            List<Ownership> list = new List<Ownership>();

            if (Dictionary == null)
                return list;

            foreach (var c in Dictionary.Keys)
            {
                foreach (var t in Dictionary[c].Keys)
                {
                    list.AddRange(Dictionary[c][t].Values.ToList());
                }
            }

            return list;
        }
    
        public static int GetTotalAssetsOfCompany(string tag)
        {
            if (Dictionary.TryGetValue(tag, out var company))
            {
                return company.Values.Sum(x => x.Values.Sum(o => o.shares));
            }
            return 0;
        }

        public static int GetTotalAssetsOfCompany(string tag, string type)
        {
            if (Dictionary.TryGetValue(tag, out var company))
            {
                if (company.TryGetValue(type, out var types))
                {
                    return types.Values.Sum(x => x.shares);
                }
            }
            return 0;
        }

        public static List<Ownership> GetAssetsOfCompany(string tag)
        {
            List<Ownership> list = new List<Ownership>();
            if (Dictionary.TryGetValue(tag, out var value))
            {
                foreach (var key in value.Keys)
                {
                    list.AddRange(value[key].Values.ToList());
                }
            }
            return list;
        }

        public static List<Ownership> GetAssetsOfCompany(string tag, string type)
        {
            if (Dictionary.TryGetValue(tag, out var value))
            {
                if (value.TryGetValue(type, out var types))
                {
                    return types.Values.ToList();
                }
            }
            return new List<Ownership>();
        }
    }
}
