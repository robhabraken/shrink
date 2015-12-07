
namespace robhabraken.SitecoreShrink
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Archiving;
    using Sitecore.Data.Items;
    using Sitecore.Resources.Media;
    using Sitecore.SecurityModel;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Utility class that offers multiple ways of cleaning up unused items.
    /// </summary>
    public class TidyUp
    {

        private Database database;

        public TidyUp(string databaseName)
        {
            this.database = Factory.GetDatabase(databaseName);
        }

        /// <summary>
        /// Saves the media files of the given items to disk, using the folder structure of the media library.
        /// </summary>
        /// <param name="items">A list of items to download.</param>
        /// <param name="targetPath">The target location for the items to be downloaded to.</param>
        public void Download(List<Item> items, string targetPath)
        {
            foreach (var item in items)
            {
                if (item != null)
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
        }

        /// <summary>
        /// Returns the file path to be used by the OS that corresponds to the location in the media library, starting at the given target path.
        /// </summary>
        /// <param name="targetPath">The starting path to store the media item in.</param>
        /// <param name="mediaPath">The Sitecore media path of the media item.</param>
        /// <param name="extension">The extensions of the Sitecore media item.</param>
        /// <returns></returns>
        private string MediaToFilePath(string targetPath, string mediaPath, string extension)
        {
            if(mediaPath.StartsWith("/"))
            {
                mediaPath = mediaPath.Substring(1);
            }
            
            return Path.Combine(targetPath, string.Format("{0}.{1}", mediaPath, extension));
        }

        /// <summary>
        /// Archives items to the archive of the current database, if an archive is configured and available.
        /// </summary>
        /// <remarks>
        /// This applies to all versions and all languages of these items.
        /// </remarks>
        /// <param name="items">A list of items to archive.</param>
        /// <param name="archiveChildren">If set to true, child items will be archived as well; if set to false, items with children will be left untouched.</param>
        public void Archive(List<Item> items, bool archiveChildren)
        {
            var archive = ArchiveManager.GetArchive("archive", database);

            if (archive != null)
            {
                using (new SecurityDisabler())
                {
                    foreach (var item in items)
                    {
                        if (item != null)
                        {
                            if (!item.HasChildren || archiveChildren)
                            {
                                archive.ArchiveItem(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes items by moving them to the recycle bin.
        /// </summary>
        /// <remarks>
        /// This applies to all versions and all languages of these items.
        /// </remarks>
        /// <param name="items">A list of items to recycle.</param>
        /// <param name="recycleChildren">If set to true, child items will be recycled as well; if set to false, items with children will be left untouched.</param>
        public void Recycle(List<Item> items, bool recycleChildren)
        {
            using (new SecurityDisabler())
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        if (!item.HasChildren || recycleChildren)
                        {
                            item.RecycleChildren();
                            item.Recycle();
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// Deletes items permanently, bypassing the recycle bin.
        /// </summary>
        /// <remarks>
        /// This applies to all versions and all languages of these items.
        /// 
        /// Note that deleting an item without deleting its children will result in the item being transformed to a media folder;
        /// this orphans the media blob (which was the goal of deleting the item), without breaking the path to underlying items.
        /// </remarks>
        /// <param name="items">A list of items to delete.</param>
        /// <param name="deleteChildren">If set to true, the underlying child items of each item will be deleted too.</param>
        public void Delete(List<Item> items, bool deleteChildren)
        {
            using (new SecurityDisabler())
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        if (item.HasChildren && deleteChildren)
                        {
                            // if the item has children and they should be deleted, do so
                            item.DeleteChildren();
                            item.Delete();
                        }
                        else if (item.HasChildren && !deleteChildren)
                        {
                            // if the item has children that should not be deleted, change the item to a folder
                            this.ChangeToFolder(item);
                        }
                        else
                        {
                            // if the item doesn't have children, the deleteChildren parameter isn't relevant, just delete the item
                            item.Delete();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Changes a media item into a media folder by changing the template of the item.
        /// This orphans the media blob so we can clean it afterwards, without breaking the path to underlying items.
        /// </summary>
        /// <param name="item">The item to change to a folder.</param>
        private void ChangeToFolder(Item item)
        {
            var mediaFolderGuid = new ID(MediaItemUsage.MEDIA_FOLDER_TEMPLATE_ID);
            var mediaFolder = database.GetItem(mediaFolderGuid);

            item.Editing.BeginEdit();
            item.ChangeTemplate(mediaFolder);
            if (item.Fields["__Icon"] != null)
            {
                item.Fields["__Icon"].Reset();
            }
            item.Editing.EndEdit();
        }

        /// <summary>
        /// Deletes all versions of all languages except the latest version of each language and the current valid version of that language.
        /// </summary>
        /// <remarks>
        /// For most items the latest version of an item will be the current valid version as well, but when cleaning up old versions,
        /// you wouldn't want to delete a valid version if you are working on a newer but not yet publishable version.
        /// 
        /// Note that publishing target settings are shared across both versions and languages, so an item is either publishable to a specific target, or not.
        /// Getting the valid version can be done without consulting each separate publishing target, because if there _is_ a valid version for one of more targets,
        /// we do not want to delete it, and if there isn't a valid version _at all_ we can ignore the publishing settings and delete all versions but the last.
        /// </remarks>
        /// <param name="items">A list of items to delete the old versions of.</param>
        public void DeleteOldVersions(List<Item> items)
        {
            using (new SecurityDisabler())
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        this.DeleteOldVersions(item);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes all versions of all languages except the latest version of each language and the current valid version of that language.
        /// </summary>
        /// <remarks>
        /// This is the equivalent to item.Archive, item.Recycle and item.Delete method, so it works on a single item only and does not handle security.
        /// </remarks>
        /// <param name="item">The item to delete the old versions of.</param>
        private void DeleteOldVersions(Item item)
        {
            foreach (var language in item.Languages)
            {
                var languageItem = database.GetItem(item.ID, language);
                var validVersion = languageItem.Publishing.GetValidVersion(DateTime.Now, true, false);

                foreach (var version in languageItem.Versions.GetVersions())
                {
                    // delete everything but the latest version and the current valid version for this language
                    if (!version.Versions.IsLatestVersion() && version.Version.Number != validVersion.Version.Number)
                    {
                        version.Versions.RemoveVersion();
                    }
                }
            }
        }

        public void CleanUpOrphanedBlobs()
        {
            throw new NotImplementedException();
        }
    }
}
