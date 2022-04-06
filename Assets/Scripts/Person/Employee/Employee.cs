using Job;
using Company;
using Employment;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Person.Employee
{
    [MoonSharpUserData]
    public class Employee : Person
    {
        private static int id_count;
        public int id;
        
        [JsonProperty("job")]
        public string job_tag;

        public int level;

        [JsonIgnore]
        public string Name => $"{name} {surname}";

        [JsonIgnore]
        public Job.Job Job => JobDictionary.Jobs[job_tag];
        [JsonIgnore]
        public float Salary => Job.levels[level].salary;

        public Employee()
        {
            id = id_count++;
            EmploymentManager.Instance.OnEmployed += HandleEmployment;
        }

        [JsonConstructor]
        public Employee(int id, string job)
        {
            this.id = id;
            job_tag = job;
            id_count++;
            EmploymentManager.Instance.OnEmployed += HandleEmployment;
        }

        ~Employee()
        {
            EmploymentManager.Instance.OnEmployed -= HandleEmployment;
        }

        private void HandleEmployment(object sender, OnEmployedData data)
        {
            if (data.employee != id) return;

            Company.Company company = CompanyDictionary.Dictionary[data.company];

            if (data.hired)
                Job.OnHired(company, this);
            else
                Job.OnFired(company, this);
        }

        public override string ToString()
        {
            string s;
            s = $"{id} | {name} {surname} | Job: {Job.title} Level: {level} Salary: {Salary}";
            return s;
        }

        public string EffectsToString()
        {
            string s = "";
            foreach (var effect in Job.levels[level].effects)
            {
                s += $"{effect.amount:+#;-#;0} {Language.LanguageManager.Instance.Translate(effect.name)}\n";
            }
            s.TrimEnd(System.Environment.NewLine.ToCharArray());
            return s;
        }

        public static Employee Create()
        {
            Employee employee = new Employee();
            employee.name = Constants.Values.names[UnityEngine.Random.Range(0, Constants.Values.names.Count)];
            employee.surname = Constants.Values.surnames[UnityEngine.Random.Range(0, Constants.Values.surnames.Count)];
            return employee;
        }
    }
}
