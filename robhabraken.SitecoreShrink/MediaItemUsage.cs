
namespace robhabraken.SitecoreShrink
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using Sitecore.Data.Archiving;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MediaItemUsage
    {
        public const string MEDIA_FOLDER_ID = "{FE5DD826-48C6-436D-B87A-7C4210C7413B}";

        public void Scan()
        {
            this.MediaItemCount = 0;
            this.UnusedItems = new List<Item>();
            this.UnpublishedItems = new List<Item>();

            //TODO: check if type is a good choice (could be item list as well)
            this.OldVersions = new List<Version>();

            //TODO: make master database configurable?
            var database = Factory.GetDatabase("master");
            var publishingHelper = new PublishingHelper();

            var root = database.Items["/sitecore/media library"];
            var descendants = root.Axes.GetDescendants();
            foreach (var item in descendants)
            {
                if (!item.Template.ID.ToString().Equals(MediaItemUsage.MEDIA_FOLDER_ID))
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
                    
                    if (!used)
                    {
                        this.UnusedItems.Add(item);
                    }
                    
                    if (publishingHelper.ListPublishedTargets(item).Count == 0)
                    {
                        this.UnpublishedItems.Add(item);
                    }

                    //TODO: find versions that are old and or obsolete
                }
            }
        }

        public int MediaItemCount { get; set; }

        public List<Item> UnusedItems { get; set; }

        public List<Item> UnpublishedItems { get; set;  }

        //TODO: fill this list
        public List<Version> OldVersions { get; set; }

    }
}
