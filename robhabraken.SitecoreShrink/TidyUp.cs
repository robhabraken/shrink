
namespace robhabraken.SitecoreShrink
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using Sitecore.Data.Archiving;
    using Sitecore.SecurityModel;
    using Sitecore.Resources.Media;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Sitecore.Data;

    public class TidyUp
    {
        //TODO: auto publish after clean up? Or should I make this optional?

        private Database database;

        public TidyUp(string databaseName)
        {
            this.database = Factory.GetDatabase(databaseName);
        }

        public void Download(List<Item> items, string targetPath)
        {
            foreach (var item in items)
            {
                var mediaItem = (MediaItem)item;
                var media = MediaManager.GetMedia(mediaItem);
                var stream = media.GetStream();

                var fullPath = this.MediaToFilePath(targetPath, mediaItem.MediaPath, mediaItem.Extension);

                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                using (var targetStream = File.OpenWrite(fullPath))
                {
                    stream.CopyTo(targetStream);
                    targetStream.Flush();
                }
            }
        }

        private string MediaToFilePath(string targetPath, string mediaPath, string extension)
        {
            if(mediaPath.StartsWith("/"))
            {
                mediaPath = mediaPath.Substring(1);
            }
            
            return Path.Combine(targetPath, string.Format("{0}.{1}", mediaPath, extension));
        }

        public void Archive(List<Item> items)
        {
            var archive = ArchiveManager.GetArchive("archive", database);

            if (archive != null)
            {
                foreach (var item in items) // check for children first?
                {
                    archive.ArchiveItem(item);
                }
            }
        }

        public void Recycle(List<Item> items)
        {
            using (new SecurityDisabler()) // or just don't do this and let the user log in and determine if he has the right rights?
            {
                foreach (var item in items) // check for children first?
                {
                    item.Recycle(); 
                }
            }            
        }

        public void Delete(List<Item> items)
        {
            using (new SecurityDisabler()) // or just don't do this and let the user log in and determine if he has the right rights?
            {
                foreach (var item in items) // check for children first?
                {
                    item.Delete();
                }
            }
        }

        public void DeleteOldVersions()
        {
            // item.Versions.RemoveVersion();
            throw new NotImplementedException();
        }

        public void CleanUpOrphanedBlobs()
        {
            throw new NotImplementedException();
        }
    }
}
