namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using System.Collections.Generic;

    public interface ITidy
    {
        void Download(string[] items, string targetPath, bool deleteAfterwards);

        void Archive(string[] items, bool archiveChildren);

        void Recycle(string[] items, bool recycleChildren);

        void Delete(string[] items, bool deleteChildren);

        void DeleteOldVersions(string[] items);
    }
}
