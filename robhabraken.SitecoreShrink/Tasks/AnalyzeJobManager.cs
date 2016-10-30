namespace robhabraken.SitecoreShrink.Tasks
{
    using Sitecore;
    using Sitecore.Jobs;
    using System;

    /// <summary>
    /// Utility class that scans the media library, analyzing its usage and size, executed as a Sitecore job.
    /// </summary>
    public class AnalyzeJobManager : IAnalyze
    {
        public const string JobCategory = "analyzing";

        private MediaAnalyzer mediaAnalyzer;

        public AnalyzeJobManager()
        {
            this.mediaAnalyzer = new MediaAnalyzer();
        }

        public void ScanMediaLibrary()
        {
            var action = "Scanning_media_library";
            var jobName = string.Format(JobInfo.JobNameFormat, JobInfo.JobType, action);

            var jobOptions = new JobOptions(
                jobName,
                AnalyzeJobManager.JobCategory,
                Context.Site.Name,
                this.mediaAnalyzer,
                "ScanMediaLibrary",
                new object[0])
            {
                AfterLife = TimeSpan.FromMinutes(5)
            };

            var job = JobManager.Start(jobOptions);
        }
    }
}