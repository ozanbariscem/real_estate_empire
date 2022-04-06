using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Job
{
    [MoonSharpUserData]
    public class JobDictionary
    {
        public static Dictionary<string, Job> Jobs;

        public static void LoadJobs(List<Job> jobs)
        {
            if (Jobs == null)
                Jobs = new Dictionary<string, Job>();
            
            foreach (var job in jobs)
            {
                if (Jobs.TryGetValue(job.tag, out Job _job))
                {
                    _job = job;
                } else
                {
                    Jobs.Add(job.tag, job);
                }
            }
        }
    }
}
