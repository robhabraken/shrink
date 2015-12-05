using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Archiving;
using Sitecore.SecurityModel;

namespace robhabraken.Sitecore.Shrink
{
    using global::Sitecore.Resources.Media;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TidyUp
    {

        public void Download(List<Item> items)
        {
            //TODO: make master database configurable?
            var database = Factory.GetDatabase("master");

            foreach (var item in items)
            {
                var mediaItem = (MediaItem)item;
                var media = MediaManager.GetMedia(mediaItem);
                var stream = media.GetStream();
                var path = Path.Combine("D:\\", string.Format("{0}.{1}", mediaItem.MediaPath, mediaItem.Extension));   //TODO: create folders that do not yet exist
                using (var targetStream = File.OpenWrite(path))
                {
                    stream.CopyTo(targetStream);
                    targetStream.Flush();
                }
            }
        }

        public void Archive(List<Item> items)
        {
            //TODO: make master database configurable?
            var database = Factory.GetDatabase("master");
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
            using (new SecurityDisabler())
            {
                foreach (var item in items) // check for children first?
                {
                    item.Editing.BeginEdit(); // do I need to do this?
                    item.Recycle();
                    // to recycle an individual version: item.RecycleVersion();
                    // to bypass the recycle bin for a version: item.Versions.RemoveVersion();
                    item.Editing.EndEdit(); // do I need to do this?
                }
            }            
        }

        public void Delete(List<Item> items)
        {
            using (new SecurityDisabler())
            {
                foreach (var item in items) // check for children first?
                {
                    item.Editing.BeginEdit();
                    item.Delete();
                    item.Editing.EndEdit();
                }
            }
        }

        public void DeleteOldVersions()
        {
            throw new NotImplementedException();
        }

        public void CleanUpOrphanedBlobs()
        {
            throw new NotImplementedException();
        }
    }
}
