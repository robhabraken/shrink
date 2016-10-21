namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using Helpers;
    using IO;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Jobs;
    using Sitecore.Links;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Utility class that is able to scan the media library to analyze item usage and space consumption. (RECOMPOSE THIS DESCRIPTION)
    /// </summary>
    public class MediaAnalyzer : IAnalyze
    {
        private Database database;

        public MediaAnalyzer(string databaseName)
        {
            this.database = Factory.GetDatabase(databaseName);
        }

        public MediaItemReport MediaItemRoot { get; set; }

        public void ScanMediaLibraryJob()
        {
            var action = "ScanMediaLibrary";
            var jobName = string.Format("{0}_{1}", this.GetType(), action);

            var jobOptions = new JobOptions(
                jobName,
                "Scanning media",
                Context.Site.Name,
                this,
                action,
                new object[0])
            {
                AfterLife = TimeSpan.FromMinutes(30)
            };

            var job = JobManager.Start(jobOptions);
        }

        public void ScanMediaLibrary()
        {
            var stopwatch = Stopwatch.StartNew();

            var root = database.Items["/sitecore/media library"];
            this.MediaItemRoot = new MediaItemReport(root);

            this.ScanItemsOf(root, this.MediaItemRoot);

            stopwatch.Stop();
            var elapsedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fffff");

            // TEMP WRITE TO JSON STUFF FOR TESTING PURPOSES
            var json = new SomethingJSON(@"D:\test.json");
            json.Serialize(this.MediaItemRoot);
        }

        private void ScanItemsOf(Item sitecoreItem, MediaItemReport reportItem)
        {
            this.Analyze(sitecoreItem, reportItem);

            if (Context.Job != null)
            {
                Context.Job.Status.Processed++;
            }

            if (sitecoreItem.HasChildren)
            {
                foreach (Item child in sitecoreItem.Children)
                {
                    var childItem = new MediaItemReport(child);
                    reportItem.Children.Add(childItem);

                    this.ScanItemsOf(child, childItem);
                }
            }
        }

        private void Analyze(Item sitecoreItem, MediaItemReport reportItem)
        {
            if (reportItem.IsMediaFolder.HasValue && !reportItem.IsMediaFolder.Value)
            {
                // update and get referrers
                Globals.LinkDatabase.UpdateReferences(sitecoreItem);

                ItemLink[] itemReferrers = null;
                try
                {
                    itemReferrers = Globals.LinkDatabase.GetReferrers(sitecoreItem);

                    // check validity of all referrers
                    foreach (var itemLink in itemReferrers)
                    {
                        if (itemLink != null)
                        {
                            var referencedItem = itemLink.GetSourceItem();
                            if (referencedItem != null)
                            {
                                reportItem.IsReferenced = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    //this.report(string.Format("Skipping this item because retrieving referrers failed due to {0}", exception.Message));
                    return;
                }

                reportItem.IsPublished = new PublishingHelper().ListPublishedTargets(sitecoreItem).Count > 0;
                reportItem.HasOldVersions = this.HasMultipleVersions(sitecoreItem);
            }
        }

        /// <summary>
        /// Checks if one or more language items of an item have more than one version.
        /// </summary>
        /// <remarks>
        /// Note that it could be that an older verions is still in use due to custom publishing restrictions,
        /// but checking that at this stage would be to costly, so we assume this items is an item to check when deleting old versions.
        /// </remarks>
        /// <param name="item">The item to check for multiple versions.</param>
        /// <returns>True if one or more language items have multiple verions.</returns>
        private bool HasMultipleVersions(Item item)
        {
            foreach (var language in item.Languages)
            {
                var languageItem = database.GetItem(item.ID, language);
                if (languageItem.Versions.Count > 1)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
