using System;
using System.Linq;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Person.Employee
{
    [MoonSharpUserData]
    public class EmployeeDictionary
    {
        public static event EventHandler<Employee> OnAddedToPool;
        public static event EventHandler<Employee> OnRemovedFromPool;

        public static Dictionary<int, Employee> Employees { get; private set; }
        public static Dictionary<string, Dictionary<int, Employee>> EmployeePool { get; private set; }

        public static Employee SafeGet(int id)
        {
            if (Employees.TryGetValue(id, out var employee)) return employee;
            return null;
        }

        public static void LoadEmployees(List<Employee> employees)
        {
            if (Employees == null)
                Employees = new Dictionary<int, Employee>();

            foreach (Employee employee in employees)
            {
                if (Employees.TryGetValue(employee.id, out var _employee))
                {
                    _employee = employee;
                } else
                {
                    Employees.Add(employee.id, employee);
                }
            }
        }
    
        public static List<Employee> SafeGetEmployees(List<int> ids)
        {
            if (Employees == null)
                return new List<Employee>();

            return Employees.Values.Where(x => ids.Contains(x.id)).ToList();
        }
    
        public static List<Employee> GetPoolWithTag(string tag)
        {
            if (EmployeePool != null)
                if (EmployeePool.TryGetValue(tag, out var dict))
                    return dict.Values.ToList();
            return new List<Employee>();
        }
            
        public static void AddToPool(Employee employee)
        {
            if (EmployeePool == null)
                EmployeePool = new Dictionary<string, Dictionary<int, Employee>>();

            if (EmployeePool.TryGetValue(employee.job_tag, out var dict))
            {
                if (dict.TryGetValue(employee.id, out var _employee))
                    _employee = employee;
                else
                    dict.Add(employee.id, employee);
            } else
            {
                EmployeePool.Add(employee.job_tag, new Dictionary<int, Employee> {{ employee.id, employee }});
            }
            OnAddedToPool?.Invoke(null, employee);
        }

        public static bool RemoveFromPool(Employee employee)
        {
            if (EmployeePool == null)
                EmployeePool = new Dictionary<string, Dictionary<int, Employee>>();

            if (EmployeePool.TryGetValue(employee.job_tag, out var dict))
                if (dict.Remove(employee.id))
                {
                    OnRemovedFromPool?.Invoke(null, employee);
                    return true;
                }
            return false;
        }
    }
}
