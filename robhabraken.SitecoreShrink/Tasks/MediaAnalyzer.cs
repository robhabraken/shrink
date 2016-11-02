using System.Linq;

namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using Helpers;
    using IO;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utility class that scans the media library, analyzing its usage and size.
    /// </summary>
    public class MediaAnalyzer : IAnalyze
    {
        private readonly Database database;
        private readonly List<string> codeReferencedItemList;

        /// <summary>
        /// Constructs an analyzer object to scan the media library of the database configured in the App.config.
        /// </summary>
        public MediaAnalyzer()
        {
            var databaseName = Settings.GetSetting("Shrink.DatabaseToScan");
            this.database = Factory.GetDatabase(databaseName);

            var codeReferencedItemIDs = Settings.GetSetting("Shrink.CodeReferencedItems");
            this.codeReferencedItemList = codeReferencedItemIDs.Split('|').ToList();
        }

        /// <summary>
        /// Constructs an analyzer object to scan the media library of the given database.
        /// </summary>
        /// <param name="databaseName">The name of the database of which to scan the media library of.</param>
        public MediaAnalyzer(string databaseName)
        {
            this.database = Factory.GetDatabase(databaseName);
        }

        /// <summary>
        /// The media item report representing the media library Sitecore root item.
        /// </summary>
        public MediaItemReport MediaItemRoot { get; set; }

        /// <summary>
        /// Scans the media library and all of its media items recursively.
        /// </summary>
        public void ScanMediaLibrary()
        {
            if (database != null)
            {
                var root = database.Items["/sitecore/media library"];
                this.MediaItemRoot = new MediaItemReport(root);

                this.ScanItemsOf(root, this.MediaItemRoot);
                this.WriteReportsToDataStorage();
            }
        }

        /// <summary>
        /// Recursive method to analyze a Sitecore media item and its child items.
        /// </summary>
        /// <param name="sitecoreItem">The Sitecore item to analyze.</param>
        /// <param name="reportItem">The media item report item to store the results and its children in.</param>
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

        /// <summary>
        /// Analyzes a Sitecore media item and stores the result in the corresponding media item report object.
        /// </summary>
        /// <param name="sitecoreItem">The Sitecore item to analyze.</param>
        /// <param name="reportItem">The media item report object to store the results in.</param>
        private void Analyze(Item sitecoreItem, MediaItemReport reportItem)
        {
            if (sitecoreItem != null && reportItem.IsMediaFolder.HasValue && !reportItem.IsMediaFolder.Value)
            {
                // update and get referrers
                Globals.LinkDatabase.UpdateReferences(sitecoreItem);

                try
                {
                    // start with the assumption that the item isn't referenced
                    reportItem.IsReferenced = false;

                    var itemReferrers = Globals.LinkDatabase.GetReferrers(sitecoreItem);

                    // check validity of all referrers
                    foreach (var itemLink in itemReferrers)
                    {
                        if (itemLink != null)
                        {
                            var referencedItem = itemLink.GetSourceItem();
                            if (referencedItem != null)
                            {
                                // if at least one valid referrer is found, report and break out
                                reportItem.IsReferenced = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    // exception handling because getting the referrers doesn't always seem to go flawless
                    Log.Error($"Shrink: exception during getting the referrers for item {{{reportItem.ID}}}", exception, this);

                    // if getting the referrers fails, the nullable IsReferenced boolean of the report object is set to null
                    // to indicate that the reference state couldn't be determined
                    reportItem.IsReferenced = null;
                }

                // if the item is referenced from code, always mark it as being referenced
                if (this.codeReferencedItemList.Contains(sitecoreItem.ID.ToString()))
                {
                    reportItem.IsReferenced = true;
                }

                // add other meta data as well
                reportItem.IsPublished = new PublishingHelper().ListPublishedTargets(sitecoreItem).Count > 0;
                reportItem.HasOldVersions = this.HasMultipleVersions(sitecoreItem);
            }
        }

        /// <summary>
        /// Checks if one or more language items of the given item have more than one version.
        /// </summary>
        /// <remarks>
        /// Note that it could be that an older verions is still in use due to custom publishing restrictions, but checking that at this stage would be to costly,
        /// so we assume this item is an item to check in close detail when deleting old versions (this is done by the TidyUp class in code, not manually!).
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

        /// <summary>
        /// Writes the resulting report data to the configured JSON files.
        /// </summary>
        private void WriteReportsToDataStorage()
        {
            var mediaItemPath = Settings.GetSetting("Shrink.MediaItemReportPath");
            if (!string.IsNullOrEmpty(mediaItemPath))
            {
                var json = new JsonStorage(mediaItemPath);
                json.Serialize(this.MediaItemRoot);
            }

            var libraryReportPath = Settings.GetSetting("Shrink.MediaLibraryReportPath");
            if (!string.IsNullOrEmpty(libraryReportPath))
            {
                var json = new JsonStorage(libraryReportPath);
                json.Serialize(new MediaLibraryReport(this.MediaItemRoot));
            }
        }
    }
}
