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
        public static event EventHandler<List<Ownership>> OnOwnershipsAdded;

        public static Dictionary<string, Dictionary<Investment.Type, Dictionary<int, Ownership>>> CompanyCategorized { get; private set; }
        public static Dictionary<Investment.Type, Dictionary<int, Ownership>> InvestmentCategorized { get; private set; }

        public static void AddOwnership(Ownership ownership)
        {
            if (CompanyCategorized == null || InvestmentCategorized == null)
            {
                CompanyCategorized = new Dictionary<string, Dictionary<Investment.Type, Dictionary<int, Ownership>>>();
                InvestmentCategorized = new Dictionary<Investment.Type, Dictionary<int, Ownership>>();

                foreach (Investment.Type type in Enum.GetValues(typeof(Investment.Type)))
                {
                    InvestmentCategorized.Add(type, new Dictionary<int, Ownership>());
                }
            }

            if (InvestmentOwner(ownership.type, ownership.investment) == null)
            {
                if (CompanyCategorized.TryGetValue(ownership.company, out var companyDict))
                {
                    if (companyDict.TryGetValue(ownership.type, out var type))
                        type.Add(ownership.investment, ownership);

                } else
                {
                    CompanyCategorized.Add(ownership.company, new Dictionary<Investment.Type, Dictionary<int, Ownership>>());
                    foreach (Investment.Type type in Enum.GetValues(typeof(Investment.Type)))
                    {
                        CompanyCategorized[ownership.company].Add(type, new Dictionary<int, Ownership>());

                        if (type == ownership.type)
                            CompanyCategorized[ownership.company][type].Add(ownership.investment, ownership);
                    }
                }

                if (InvestmentCategorized[ownership.type].TryGetValue(ownership.investment, out var _ownership))
                {
                    _ownership = ownership;
                } else
                {
                    InvestmentCategorized[ownership.type].Add(ownership.investment, ownership);
                }
                OnOwnershipAdded?.Invoke(null, ownership);
            } else
            {
                Console.Console.Run($"log_error Tried to add ownership of an investment that already belongs to another company!");
                Console.Console.Run($"log_error -- To change ownership of investments please use {nameof(OwnershipDictionary)}.{nameof(ChangeOwnership)} function.");
            }
        }

        public static void ChangeOwnership(Ownership ownership)
        {

        }

        public static void LoadOwnerships(List<Ownership> ownerships)
        {
            foreach (var ownership in ownerships)
                AddOwnership(ownership);
            OnOwnershipsAdded?.Invoke(null, ownerships);
        }
    
        public static bool CompanyOwns(Company.Company company, Investment.Type type, int id)
        {
            return InvestmentOwner(type, id) == company.tag;
        }

        public static string InvestmentOwner(Investment.Type type, int id)
        {
            if (InvestmentCategorized != null )
                if (InvestmentCategorized.TryGetValue(type, out var dict))
                    if (dict.TryGetValue(id, out var ownership)) return ownership.company;
            return null;
        }
    
        public static List<Ownership> ToList()
        {
            return InvestmentCategorized.Values.SelectMany(x => x.Values).ToList();
        }
    }
}
