namespace robhabraken.SitecoreShrink.Tasks
{
    using Sitecore.Jobs;
    using System.Collections.Generic;
    using System.Linq;

    public class JobInfo
    {
        public const string JobNameFormat = "{0}:{1}";
        public const string JobType = "robhabraken.SitecoreShrink.Jobs";

        public string GetFriendlyJobName(string jobName)
        {
            return jobName
                .Replace(JobInfo.JobType, string.Empty)
                .Replace(":", string.Empty)
                .Replace("_", " ") + "...";
        }

        public IEnumerable<Job> Jobs
        {
            get
            {
                return JobManager.GetJobs().Where(job => !job.IsDone).OrderBy(job => job.QueueTime);
            }
        }
    }
}