
namespace robhabraken.SitecoreShrink.Tasks
{
    using Sitecore.Data.Items;
    using System.Collections.Generic;

    public interface ITidy
    {
        void Download(List<Item> items, string targetPath, bool deleteAfterwards);

        void Archive(List<Item> items, bool archiveChildren);

        void Recycle(List<Item> items, bool recycleChildren);

        void Delete(List<Item> items, bool deleteChildren);

        void DeleteOldVersions(List<Item> items);
    }
}
