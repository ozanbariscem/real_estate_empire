using System.Collections.Generic;
using MoonSharp.Interpreter;
using System.Linq;

namespace Company
{
    [MoonSharpUserData]
    public class CompanyDictionary
    {
        public static Dictionary<string, Company> Dictionary { get; private set; }

        public static void AddCompanies(List<Company> companies)
        {
            if (Dictionary == null)
                Dictionary = new Dictionary<string, Company>();

            foreach (var company in companies)
            {
                if (Dictionary.TryGetValue(company.tag, out var _company))
                {
                    _company = company;
                }
                else
                {
                    Dictionary.Add(company.tag, company);
                }
            }
        }
    
        public static List<Company> GetAsList()
        {
            return Dictionary.Values.ToList();
        }
    }
}
