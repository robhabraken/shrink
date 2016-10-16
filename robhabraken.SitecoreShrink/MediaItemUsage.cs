
namespace robhabraken.SitecoreShrink
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Links;
    using System;
    using Sitecore.Jobs;
    using System.Diagnostics;

    /// <summary>
    /// Utility class that is able to scan the media library for unused items.
    /// </summary>
    public class MediaItemUsage
    {
        public const string MEDIA_FOLDER_TEMPLATE_ID = "{FE5DD826-48C6-436D-B87A-7C4210C7413B}";

        private Database database;
        private MediaItemReport itemReport;

        public MediaItemUsage(string databaseName)
        {
            this.database = Factory.GetDatabase(databaseName);
        }

        // temp file log (unoptimized)
        private void report(string line)
        {
            //using (var file = new System.IO.StreamWriter(@"D:\report.txt", true))
            //{
            //    file.WriteLine(line);
            //}
        }

        public void ScanMediaLibraryOptimizedJob()
        {
            var action = "ScanMediaLibraryOptimized";
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

        // because original scan function was too slow and bulky using the Descendants call, I'm working on a recursive method ATM
        public void ScanMediaLibraryOptimized()
        {
            var stopwatch = Stopwatch.StartNew();

            this.itemReport = new MediaItemReport();

            var root = database.Items["/sitecore/media library"];
            this.ScanItemsOf(root);

            stopwatch.Stop();
            var elapsedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fffff");

            // TEMP TEMP TEMP TEMP TEMP
            using (var file = new System.IO.StreamWriter(@"D:\report.txt", true))
            {
                file.WriteLine(string.Format("Items processed: {0}, items unused: {1}, items unpublished: {2}, items with old versions: {3}, total size of unused items: {4}", this.itemReport.MediaItemCount, this.itemReport.UnusedItems.Count, this.itemReport.UnpublishedItems.Count, this.itemReport.OldVersions.Count, this.itemReport.UnusedSize));
                file.WriteLine(elapsedTime);
            }
        }

        private void ScanItemsOf(Item item)
        {
            this.Evaluate(item);

            if (Context.Job != null)
            {
                Context.Job.Status.Processed++;
            }

            if (item.HasChildren)
            {
                foreach (Item child in item.Children)
                {
                    this.ScanItemsOf(child);
                }
            }
        }

        private void Evaluate(Item item)
        {
            if (!item.Template.ID.ToString().Equals(MediaItemUsage.MEDIA_FOLDER_TEMPLATE_ID))
            {
                // count all items that are actually media items (not folders)
                this.itemReport.MediaItemCount++;

                // update and get referrers
                Globals.LinkDatabase.UpdateReferences(item);

                ItemLink[] itemReferrers = null;
                try
                {
                    itemReferrers = Globals.LinkDatabase.GetReferrers(item);
                }
                catch(Exception exception)
                {
                    this.report(string.Format("Skipping this item because retrieving referrers failed due to {0}", exception.Message));
                    return;
                }

                // check validity of all referrers
                var used = false;
                foreach (var itemLink in itemReferrers)
                {
                    if (itemLink != null)
                    {
                        var referencedItem = itemLink.GetSourceItem();
                        if (referencedItem != null)
                        {
                            used = true;
                            break;
                        }
                    }
                }

                // add the item to the appropriate collections based on its state (used, published or multiple versions)
                if (!used)
                {
                    this.itemReport.UnusedItems.Add(item);

                    if (item.Paths.IsMediaItem)
                    {
                        var mediaItem = (MediaItem)item;
                        this.itemReport.UnusedSize += mediaItem.Size;
                    }
                }

                if (new PublishingHelper().ListPublishedTargets(item).Count == 0)
                {   
                    this.itemReport.UnpublishedItems.Add(item);
                }

                if (this.HasMultipleVersions(item))
                {
                    this.itemReport.OldVersions.Add(item);
                }
            }
        }

        public MediaItemReport ScanMediaLibrary()
        {
            var itemReport = new MediaItemReport();
            
            var publishingHelper = new PublishingHelper();

            var root = database.Items["/sitecore/media library"];
            var descendants = root.Axes.GetDescendants();

            this.report(string.Format("Startig scanning {0} media library descendants", descendants.Length));

            foreach (var item in descendants)
            {
                if (!item.Template.ID.ToString().Equals(MediaItemUsage.MEDIA_FOLDER_TEMPLATE_ID))
                {
                    // count all items that are actually media items (not folders)
                    itemReport.MediaItemCount++;

                    // update and get referrers
                    Globals.LinkDatabase.UpdateReferences(item);
                    var itemReferrers = Globals.LinkDatabase.GetReferrers(item);

                    // check validity of all referrers
                    var used = false;
                    foreach (var itemLink in itemReferrers)
                    {
                        if (itemLink != null)
                        {
                            var referencedItem = itemLink.GetSourceItem();
                            if (referencedItem != null)
                            {
                                used = true;
                                break;
                            }
                        }
                    }
                    
                    // add the item to the appropriate collections based on its state (used, published or multiple versions)
                    if (!used)
                    {
                        itemReport.UnusedItems.Add(item);
                    }
                    
                    if (publishingHelper.ListPublishedTargets(item).Count == 0)
                    {
                        itemReport.UnpublishedItems.Add(item);
                    }

                    if (this.HasMultipleVersions(item))
                    {
                        itemReport.OldVersions.Add(item);
                    }

                    this.report(string.Format("Items processed: {0}, items unused: {1}, items unpublished: {2}, items with old versions: {3}", itemReport.MediaItemCount, itemReport.UnusedItems.Count, itemReport.UnpublishedItems.Count, itemReport.OldVersions.Count));
                }
                else
                {
                    this.report("This is a media folder, so we will skip this item");
                }
            }
            this.report("Ready scanning media library");

            return itemReport;
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
