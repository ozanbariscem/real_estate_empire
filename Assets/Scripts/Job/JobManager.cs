using System;
using System.Linq;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Job
{
    [MoonSharpUserData]
    public class JobManager : Manager.Manager
    {
        public static JobManager Instance { get; private set; }

        public event EventHandler<List<Job>> OnJobsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "job/manager.lua";

            LoadScript();
            LoadJobs();
            RaiseOnRulesLoaded();
        }

        private void LoadJobs()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/job/jobs.json");
            if (json == null) return;

            JobDictionary.LoadJobs(JsonConvert.DeserializeObject<List<Job>>(json));
            OnJobsLoaded?.Invoke(this, JobDictionary.Jobs.Values.ToList());
        }
    }
}
