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
        /// Returns the most important job info for the SPEAK interface to display the job description and progress bar (if applicable).
        /// </summary>
        /// <param name="job">The Sitecore job to get the info of.</param>
        /// <returns>A object array that contains the friendly job name as a string and two long values for the processed and total item count.</returns>
        public static List<object> GetJobInfo(Job job)
        {
            if(job == null)
            {
                return null;
            }

            return new List<object>()
            {
                GetFriendlyJobName(job),
                job.Status.Processed,
                job.Status.Total
            };
        }


        /// <summary>
        /// Formats the job name of a JobInfo.JobType type job into a friendly name that can be used to display the current job status.
        /// </summary>
        /// <param name="job">The Sitecore job to get the name of.</param>
        /// <returns>A formatted friendly job description.</returns>
        private static string GetFriendlyJobName(Job job)
        {
            return job.Name
                .Replace(JobType, string.Empty)
                .Replace(":", string.Empty)
                .Replace("_", " ");
        }

        /// <summary>
        /// Returns all active jobs that belong to the Sitecore Shrink module, ordered by queue time.
        /// </summary>
        public static List<Job> MyJobs
        {
            get
            {
                return JobManager.GetJobs().Where(job => !job.IsDone && job.Name.StartsWith(JobType)).OrderBy(job => job.QueueTime).ToList();
            }
        }
    }
}