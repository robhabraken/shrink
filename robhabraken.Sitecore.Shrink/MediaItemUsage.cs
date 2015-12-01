using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Archiving;

namespace robhabraken.Sitecore.Shrink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MediaItemUsage
    {
        private const string MEDIA_FOLDER_ID = "{FE5DD826-48C6-436D-B87A-7C4210C7413B}";

        public void Scan()
        {
            this.UsedItemCount = 0;
            this.PublishedItemCount = 0;
            this.UnusedItems = new List<Item>();
            this.UnpublishedItems = new List<Item>();

            //TODO: check if type is a good choice (could be item list as well)
            this.OldVersions = new List<Version>();

            var database = Factory.GetDatabase("master");
            var archive = ArchiveManager.GetArchive("archive", database);
            var publishingHelper = new PublishingHelper();

            var root = database.Items["/sitecore/media library"];
            var descendants = root.Axes.GetDescendants();
            foreach (var item in descendants)
            {
                if (!item.Template.ID.ToString().Equals(workinprogress.MEDIA_FOLDER_ID))
                {
                    // count all items that are actually media items (not folders)
                    this.MediaItemCount++;

                    // update and get referrers
                    Globals.LinkDatabase.UpdateReferences(item);
                    var itemReferrers = Globals.LinkDatabase.GetReferrers(item);

                    // find all valid referrers
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

                    // process usage results
                    if (used)
                    {
                        this.UsedItemCount++;
                    }
                    else
                    {
                        this.UnusedItems.Add(item);
                    }

                    // count targets that this item is actually published to and process results
                    if (publishingHelper.ListPublishedTargets(item).Count > 0)
                    {
                        this.PublishedItemCount++;
                    }
                    else
                    {
                        this.UnpublishedItems.Add(item);
                    }

                    //TODO: find versions that are old and or obsolete
                }
            }
        }

        public int MediaItemCount { get; set; }

        public int UsedItemCount { get; set; }

        public int UnusedItemCount
        {
            get
            {
                return this.MediaItemCount - this.UsedItemCount;
            }
        }

        public List<Item> UnusedItems { get; set; }

        public int PublishedItemCount { get; set; }

        public int UnpublishedItemCount
        {
            get
            {
                return this.MediaItemCount - this.PublishedItemCount;
            }
        }

        public List<Item> UnpublishedItems { get; set;  }

        //TODO: use this (or just count size of OldVersions list?
        public int OldVersionsItemCount { get; set; }

        //TODO: fill this list
        public List<Version> OldVersions { get; set; }

    }
}
