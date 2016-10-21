﻿namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using Sitecore;
    using Sitecore.Jobs;
    using System;
    using System.Collections.Generic;

    public class AnalyzeJobManager : IAnalyze
    {
        private MediaAnalyzer mediaAnalyzer;

        public AnalyzeJobManager(string databaseName)
        {
            this.mediaAnalyzer = new MediaAnalyzer(databaseName);
        }

        public void ScanMediaLibrary()
        {
            var action = "ScanMediaLibrary";
            var jobName = string.Format("{0}_{1}", this.GetType(), action);

            var jobOptions = new JobOptions(
                jobName,
                "Analyzing media library",
                Context.Site.Name,
                this.mediaAnalyzer,
                action,
                new object[0])
            {
                AfterLife = TimeSpan.FromMinutes(30)
            };

            var job = JobManager.Start(jobOptions);
        }
    }
}