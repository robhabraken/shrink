namespace robhabraken.SitecoreShrink.Tasks
{
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

        private readonly TidyUp tidyUp;

        public TidyJobManager()
        {
            this.tidyUp = new TidyUp();
        }

        public void Archive(List<string> itemIDs, bool archiveChildren)
        {
            var args = new object[] { itemIDs, archiveChildren };
            this.StartJob("Archiving_media", "Archive", args);
        }

        public void Delete(List<string> itemIDs, bool deleteChildren)
        {
            var args = new object[] { itemIDs, deleteChildren };
            this.StartJob("Deleting_media", "Delete", args);
        }

        public void DeleteOldVersions(List<string> itemIDs)
        {
            var args = new object[] { itemIDs };
            this.StartJob("Deleting_old_versions", "DeleteOldVersions", args);
        }

        public void Download(List<string> itemIDs, string targetPath, bool deleteAfterwards)
        {
            var args = new object[] { itemIDs, targetPath, deleteAfterwards };
            this.StartJob("Downloading_media", "Download", args);
        }

        public void Recycle(List<string> itemIDs, bool recycleChildren)
        {
            var args = new object[] { itemIDs, recycleChildren };
            this.StartJob("Recycling_media", "Recycle", args);
        }

        private void StartJob(string name, string action, object[] args)
        {
            var jobName = string.Format(JobInfo.JobNameFormat, JobInfo.JobType, name);

            var jobOptions = new JobOptions(
                jobName,
                JobCategory,
                Context.Site.Name,
                this.tidyUp,
                action,
                args)
            {
                AfterLife = TimeSpan.FromMinutes(5)
            };

            JobManager.Start(jobOptions);
        }
    }
}