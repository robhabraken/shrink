
namespace robhabraken.SitecoreShrink
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utility class that is able to scan the media library for unused items.
    /// </summary>
    public class MediaItemUsage
    {
        public const string MEDIA_FOLDER_TEMPLATE_ID = "{FE5DD826-48C6-436D-B87A-7C4210C7413B}";

        private Database database;

        public MediaItemUsage(string databaseName)
        {
            this.database = Factory.GetDatabase(databaseName);
        }

        public MediaItemReport ScanMediaLibrary()
        {
            var itemReport = new MediaItemReport();
            
            var publishingHelper = new PublishingHelper();

            var root = database.Items["/sitecore/media library"];
            var descendants = root.Axes.GetDescendants();
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
                }
            }

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
