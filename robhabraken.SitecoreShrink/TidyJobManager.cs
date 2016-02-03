
namespace robhabraken.SitecoreShrink
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Jobs;
    using System;
    using System.Collections.Generic;

    public class TidyJobManager : ITidy
    {
        private TidyUp tidyUp;

        public TidyJobManager(string databaseName)
        {
            this.tidyUp = new TidyUp(databaseName);
        }

        public void Archive(List<Item> items, bool archiveChildren)
        {
            var args = new object[] { items, archiveChildren };
            this.doSomething("Archive", args);
        }

        public void Delete(List<Item> items, bool deleteChildren)
        {
            var args = new object[] { items, deleteChildren };
            this.doSomething("Delete", args);
        }

        public void DeleteOldVersions(List<Item> items)
        {
            var args = new object[] { items };
            this.doSomething("DeleteOldVersions", args);
        }

        public void Download(List<Item> items, string targetPath, bool deleteAfterwards)
        {
            var args = new object[] { items, targetPath, deleteAfterwards };
            this.doSomething("Download", args);
        }

        public void Recycle(List<Item> items, bool recycleChildren)
        {
            var args = new object[] { items, recycleChildren };
            this.doSomething("Recycle", args);
        }

        public void doSomething(string action, object[] args)
        {
            var jobName = string.Format("{0}_{1}_Media", tidyUp.GetType(), action);

            var jobOptions = new JobOptions(
                jobName,
                "Batch clean up",
                Context.Site.Name,
                tidyUp,
                action,
                args)
            {
                AfterLife = TimeSpan.FromMinutes(30)
            };

            var job = JobManager.Start(jobOptions);
        }
    }
}
