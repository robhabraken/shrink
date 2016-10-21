namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using System.Collections.Generic;

    public interface ITidy
    {
        void Download(List<MediaItemReport> items, string targetPath, bool deleteAfterwards);

        void Archive(List<MediaItemReport> items, bool archiveChildren);

        void Recycle(List<MediaItemReport> items, bool recycleChildren);

        void Delete(List<MediaItemReport> items, bool deleteChildren);

        void DeleteOldVersions(List<MediaItemReport> items);
    }
}
