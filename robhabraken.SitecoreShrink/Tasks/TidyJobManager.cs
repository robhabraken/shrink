namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using Sitecore;
    using Sitecore.Jobs;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utility class that offers multiple ways of cleaning up unused items, executed as a Sitecore job.
    /// </summary>
    public class TidyJobManager : ITidy
    {
        public const string JobCategory = "cleaning";

        private TidyUp tidyUp;

        public TidyJobManager(string databaseName)
        {
            this.tidyUp = new TidyUp(databaseName);
        }

        public void Archive(List<MediaItemReport> items, bool archiveChildren)
        {
            var args = new object[] { items, archiveChildren };
            this.StartJob("Archiving_media", "Archive", args);
        }

        public void Delete(List<MediaItemReport> items, bool deleteChildren)
        {
            var args = new object[] { items, deleteChildren };
            this.StartJob("Deleting_media", "Delete", args);
        }

        public void DeleteOldVersions(List<MediaItemReport> items)
        {
            var args = new object[] { items };
            this.StartJob("Deleting_old_versions", "DeleteOldVersions", args);
        }

        public void Download(List<MediaItemReport> items, string targetPath, bool deleteAfterwards)
        {
            var args = new object[] { items, targetPath, deleteAfterwards };
            this.StartJob("Downloading_media", "Download", args);
        }

        public void Recycle(List<MediaItemReport> items, bool recycleChildren)
        {
            var args = new object[] { items, recycleChildren };
            this.StartJob("Recycling_media", "Recycle", args);
        }

        private void StartJob(string name, string action, object[] args)
        {
            var jobName = string.Format(JobInfo.JobNameFormat, JobInfo.JobType, name);

            var jobOptions = new JobOptions(
                jobName,
                TidyJobManager.JobCategory,
                Context.Site.Name,
                this.tidyUp,
                action,
                args)
            {
                AfterLife = TimeSpan.FromMinutes(30)
            };

            var job = JobManager.Start(jobOptions);
        }
    }
}