namespace robhabraken.SitecoreShrink.Tasks
{
    using Sitecore.Jobs;
    using System.Collections.Generic;
    using System.Linq;

    public class JobInfo
    {
        public const string JobNameFormat = "{0}:{1}";
        public const string JobType = "robhabraken.SitecoreShrink.Jobs";

        /// <summary>
        /// Formats the job name of a JobInfo.JobType type job into a friendly name that can be used to display the current job status.
        /// </summary>
        /// <param name="jobName">The Sitecore job to get the name of.</param>
        /// <returns>A formatted friendly job description.</returns>
        public static string GetFriendlyJobName(Job job)
        {
            return job.Name
                .Replace(JobInfo.JobType, string.Empty)
                .Replace(":", string.Empty)
                .Replace("_", " ");
        }

        /// <summary>
        /// Returns all active jobs that belong to the Sitecore Shrink module, ordered by queue time.
        /// </summary>
        public static List<Job> Jobs
        {
            get
            {
                return JobManager.GetJobs().Where(job => !job.IsDone && job.Name.StartsWith(JobInfo.JobType)).OrderBy(job => job.QueueTime).ToList<Job>();
            }
        }
    }
}