namespace robhabraken.SitecoreShrink.Tasks
{
    using System.Collections.Generic;

    public interface ITidy
    {
        void Download(List<string> itemIDs, string targetPath, bool deleteAfterwards);

        void Archive(List<string> itemIDs, bool archiveChildren);

        void Recycle(List<string> itemIDs, bool recycleChildren);

        void Delete(List<string> itemIDs, bool deleteChildren);

        void DeleteOldVersions(List<string> itemIDs);
    }
}
