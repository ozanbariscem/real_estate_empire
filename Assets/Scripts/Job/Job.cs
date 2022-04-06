using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Job
{
    [MoonSharpUserData]
    public class Job
    {
        public int id;
        public string tag;
        public string title;
        public string description;

        [JsonIgnore]
        public Script script;
        [JsonProperty("script")]
        public string scriptPath;

        public List<Level> levels;
        
        #region SETUP
        public Job(int id, string tag, string title, string description, string script, List<Level> levels)
        {
            this.id = id;
            this.tag = tag;
            this.title = title;
            this.description = description;
            this.levels = levels;
            scriptPath = script;

            LoadScript();
        }

        private void LoadScript()
        {
            script = Utils.StreamingAssetsHandler.SafeGetScript($"vanilla/job/scripts/{scriptPath}");
            if (script != null)
            {
                script.Call(script.Globals["OnReady"]);
            }
        }
        #endregion

        [MoonSharpHidden]
        public void OnHired(Company.Company company, Person.Employee.Employee employee)
        {
            if (script != null)
            {
                script.Call(script.Globals[nameof(OnHired)], company, employee);
            }
        }

        [MoonSharpHidden]
        public void OnFired(Company.Company company, Person.Employee.Employee employee)
        {
            if (script != null)
            {
                script.Call(script.Globals[nameof(OnFired)], company, employee);
            }
        }

        public override string ToString()
        {
            string s;

            s = $"{id} - {tag}\n";
            s += $"-- Title: {title} | Description: {description}";
            s += $"\n-- Levels:";

            int i = 0;
            foreach (var level in levels)
            {
                s += $"\n---- {i++} | Popularity: {level.popularity} | Salary: {level.salary}";
            
                foreach (var effect in level.effects)
                {
                    s += $"\n------ Effects | Name: {effect.name} | Amount: {effect.amount}";
                }
            }

            return s;
        }
    }
}

