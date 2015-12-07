
namespace robhabraken.SitecoreShrink
{
    using Sitecore.Data.Items;
    using System.Collections.Generic;

    public class MediaItemReport
    {

        public MediaItemReport()
        {
            this.MediaItemCount = 0;
            this.UnusedItems = new List<Item>();
            this.UnpublishedItems = new List<Item>();
            this.OldVersions = new List<Item>();
        }

        public int MediaItemCount { get; set; }

        public List<Item> UnusedItems { get; set; }

        public List<Item> UnpublishedItems { get; set; }

        public List<Item> OldVersions { get; set; }

    }
}
