using System.Linq;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Employment
{
    [MoonSharpUserData]
    public class EmploymentDictionary
    {
        public static Dictionary<string, Dictionary<int, Employment>> Employments { get; private set; }

        public static List<int> SafeGetEmployeeIdsOfCompany(string tag)
        {
            if (Employments != null && Employments.TryGetValue(tag, out var employees))
                return employees.Keys.ToList();
            return new List<int>();
        }

        [MoonSharpHidden]
        public static bool AddEmployment(Employment employment)
        {
            if (
                !Company.CompanyDictionary.Dictionary.ContainsKey(employment.company_tag) ||
                !Person.Employee.EmployeeDictionary.Employees.ContainsKey(employment.employee_id))
            {
                return false;
            }

            if (Employments == null)
                Employments = new Dictionary<string, Dictionary<int, Employment>>();

            if (Employments.TryGetValue(employment.company_tag, out var employments))
            {
                if (employments.TryGetValue(employment.employee_id, out var _employment))
                {
                    _employment = employment;
                } else
                {
                    Employments[employment.company_tag].Add(employment.employee_id, employment);
                }
            }
            else
            {
                Employments.Add(employment.company_tag, new Dictionary<int, Employment>() { { employment.employee_id, employment } });
            }

            return true;
        }

        [MoonSharpHidden]
        public static bool RemoveEmployment(Employment employment)
        {
            if (Employments.TryGetValue(employment.company_tag, out var employments))
            {
                return employments.Remove(employment.employee_id);
            }
            return false;
        }
    }
}
