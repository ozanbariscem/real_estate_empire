using System;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using Employment;

namespace Person.Employee
{
    [MoonSharpUserData]
    public class EmployeeManager : Manager.Manager
    {
        public static EmployeeManager Instance { get; private set; }

        public event EventHandler<Dictionary<int, Employee>> OnEmployeesLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        #region SETUP
        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "person/employee/manager.lua";

            LoadScript();
            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadEmployees(path);
            RaiseOnContentLoaded();
        }

        private void LoadEmployees(string path)
        {
            string json = Utils.ContentHandler.SafeGetString($"{path}/person/employee/employees.json");
            if (json == null) return;

            EmployeeDictionary.LoadEmployees(JsonConvert.DeserializeObject<List<Employee>>(json));
            OnEmployeesLoaded?.Invoke(this, EmployeeDictionary.Employees);
        }
        #endregion

        private void HandleEmployed(object sender, OnEmployedData data)
        {
            Employee employee = EmployeeDictionary.SafeGet(data.employee);
            if (data.hired)
            {
                // It means this employee was hired from the pool
                EmployeeDictionary.RemoveFromPool(employee);
            } else
                EmployeeDictionary.AddToPool(employee);
        }

        private void HandleMonthPass(object sender, Time.Date date)
        {
            List<Employee> employees = new List<Employee>();
            foreach (var job in Job.JobDictionary.Jobs.Values)
            {
                if (!EmployeeDictionary.EmployeePool.ContainsKey(job.tag))
                    EmployeeDictionary.EmployeePool.Add(job.tag, new Dictionary<int, Employee>());
                if (EmployeeDictionary.EmployeePool.TryGetValue(job.tag, out var dict))
                {
                    if (dict.Count < Constants.Values.available_employees_per_job)
                    {
                        int l = 0;
                        foreach (var level in job.levels)
                        {
                            var amount = level.popularity * Constants.Values.monthly_employees;
                            if (amount + dict.Count > Constants.Values.available_employees_per_job)
                                amount = Constants.Values.available_employees_per_job - dict.Count;

                            for (int i = 0; i < amount; i++)
                            {
                                Employee employee = Employee.Create();
                                employee.job_tag = job.tag;
                                employee.level = l;
                                employees.Add(employee);
                                EmployeeDictionary.AddToPool(employee);
                            }
                            l++;
                        }
                    }
                }
            }
            EmployeeDictionary.LoadEmployees(employees);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            Time.TimeManager.Instance.OnMonthPass += HandleMonthPass;
            EmploymentManager.Instance.OnEmployed += HandleEmployed;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Time.TimeManager.Instance.OnMonthPass -= HandleMonthPass;
            EmploymentManager.Instance.OnEmployed -= HandleEmployed;
        }
    }
}
