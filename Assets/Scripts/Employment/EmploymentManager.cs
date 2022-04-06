using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Employment
{
    [MoonSharpUserData]
    public class EmploymentManager : Manager.Manager
    {
        public static EmploymentManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, Dictionary<int, Employment>>> OnEmploymentsLoaded;

        public event EventHandler<OnEmployedData> OnEmployed;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "employment/manager.lua";

            LoadScript();
            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadEmployments(path);
            RaiseOnContentLoaded();
        }

        private void LoadEmployments(string path)
        {
            string json = Utils.ContentHandler.SafeGetString($"{path}/employment/employments.json");
            if (json == null) return;

            List<Employment> employments = JsonConvert.DeserializeObject<List<Employment>>(json);
            foreach (var employment in employments)
                Hire(employment.company_tag, employment.employee_id);

            OnEmploymentsLoaded?.Invoke(this, EmploymentDictionary.Employments);
        }
    
        public void Hire(string company, int employee)
        {
            if (EmploymentDictionary.AddEmployment(new Employment(company, employee)))
                OnEmployed?.Invoke(this, new OnEmployedData(company, employee, true));
            else
                Debug.Log("Hire returned false");
        }

        public void Fire(string company, int employee)
        {
            if (EmploymentDictionary.RemoveEmployment(new Employment(company, employee)))
                OnEmployed?.Invoke(this, new OnEmployedData(company, employee, false));
        }
    }
}

