namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using Sitecore.Data.Items;
    using System.Collections.Generic;

    public interface ITidy
    {
        void Download(List<MediaItemX> items, string targetPath, bool deleteAfterwards);

        void Archive(List<MediaItemX> items, bool archiveChildren);

        void Recycle(List<MediaItemX> items, bool recycleChildren);

        void Delete(List<MediaItemX> items, bool deleteChildren);

        void DeleteOldVersions(List<MediaItemX> items);
    }
}
