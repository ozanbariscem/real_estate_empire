using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Person
{
    [MoonSharpUserData]
    public class Constants
    {
        public static Constants Values { get; private set; }

        public byte available_employees_per_job;
        public byte monthly_employees;
        public List<string> names;
        public List<string> surnames;

        public Constants(
            byte available_employees_per_job, 
            byte monthly_employees, 
            List<string> names, List<string> surnames)
        {
            this.available_employees_per_job = available_employees_per_job;
            this.monthly_employees = monthly_employees;
            this.names = names;
            this.surnames = surnames;

            Values = this;
        }
    }
}

