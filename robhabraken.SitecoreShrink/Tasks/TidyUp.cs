namespace robhabraken.SitecoreShrink.Tasks
{
    using Entities;
    using Helpers;
    using IO;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Archiving;
    using Sitecore.Data.Items;
    using Sitecore.Resources.Media;
    using Sitecore.SecurityModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    /// <summary>
    /// Utility class that offers multiple ways of cleaning up unused items.
    /// </summary>
    public class TidyUp : ITidy
    {
        private Database database;

        private enum ActionType { DeleteItem, DeleteVersions }

        /// <summary>
        /// Constructs a clean up class for the database configured in the App.config.
        /// </summary>
        public TidyUp()
        {
            var databaseName = Settings.GetSetting("Shrink.DatabaseToScan");
            this.database = Factory.GetDatabase(databaseName);
        }

        /// <summary>
        /// Constructs a clean up class for the given database.
        /// </summary>
        /// <param name="databaseName">The name of the database to clean up items from.</param>
        public TidyUp(string databaseName)
        {
            this.database = Factory.GetDatabase(databaseName);
        }

        /// <summary>
        /// Saves the media files of the given items to disk, using the folder structure of the media library.
        /// </summary>
        /// <param name="itemIDs">A list of Sitecore item ID strings of the items to download.</param>
        /// <param name="targetPath">The target location for the items to be downloaded to.</param>
        /// <param name="deleteAfterwards">If set to true, the items will be deleted from the Sitecore database after the download is completed.</param>
        public void Download(List<string> itemIDs, string targetPath, bool deleteAfterwards)
        {
            if (Context.Job != null)
            {
                Context.Job.Status.Total = itemIDs.Count;
            }
            
            targetPath = string.Format("{0}{1}", HttpRuntime.AppDomainAppPath, targetPath);

            foreach (var itemId in itemIDs)
            {
                var scItem = this.database.GetItem(new ID(itemId));
                if (scItem != null && !scItem.TemplateID.ToString().Equals(MediaConstants.MediaFolderTemplateID))
                {
                    if (Context.Job != null)
                    {
                        Context.Job.Status.Messages.Add(string.Format("Downloading media of item ", scItem.Paths.FullPath));
                        Context.Job.Status.Processed++;
                    }

                    var mediaItem = (MediaItem)scItem;
                    var media = MediaManager.GetMedia(mediaItem);
                    var stream = media.GetStream();

                    if (stream != null)
                    {
                        var fullPath = this.MediaToFilePath(targetPath, mediaItem.MediaPath, mediaItem.Extension);

                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        using (var targetStream = File.OpenWrite(fullPath))
                        {
                            stream.CopyTo(targetStream);
                            targetStream.Flush();
                        }
                    }

                    if(deleteAfterwards)
                    {
                        this.DeleteItem(scItem, false);
                    }
                }
            }

            if(deleteAfterwards)
            {
                this.UpdateStorageAfterCleanUp(itemIDs, ActionType.DeleteItem);
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
        /// <param name="itemIDs">A list of Sitecore item ID strings of the items to archive.</param>
        /// <param name="archiveChildren">If set to true, child items will be archived as well; if set to false, items with children will be left untouched.</param>
        public void Archive(List<string> itemIDs, bool archiveChildren)
        {
            var archive = ArchiveManager.GetArchive("archive", database);
            if (Context.Job != null)
            {
                Context.Job.Status.Total = itemIDs.Count;
            }

            if (archive != null)
            {
                using (new SecurityDisabler())
                {
                    foreach (var itemId in itemIDs)
                    {
                        var scItem = this.database.GetItem(new ID(itemId));
                        if (scItem != null && !scItem.TemplateID.ToString().Equals(MediaConstants.MediaFolderTemplateID))
                        {
                            if (Context.Job != null)
                            {
                                Context.Job.Status.Messages.Add(string.Format("Archiving item ", scItem.Paths.FullPath));
                                Context.Job.Status.Processed++;
                            }

                            if (!scItem.HasChildren || archiveChildren)
                            {
                                archive.ArchiveItem(scItem);
                            }
                        }
                    }
                }
            }

            this.UpdateStorageAfterCleanUp(itemIDs, ActionType.DeleteItem);
        }

        /// <summary>
        /// Deletes items by moving them to the recycle bin.
        /// </summary>
        /// <remarks>
        /// This applies to all versions and all languages of these items.
        /// </remarks>
        /// <param name="itemIDs">A list of Sitecore item ID strings of the items to recycle.</param>
        /// <param name="recycleChildren">If set to true, child items will be recycled as well; if set to false, items with children will be left untouched.</param>
        public void Recycle(List<string> itemIDs, bool recycleChildren)
        {
            if (Context.Job != null)
            {
                Context.Job.Status.Total = itemIDs.Count;
            }

            using (new SecurityDisabler())
            {
                foreach (var itemId in itemIDs)
                {
                    var scItem = this.database.GetItem(new ID(itemId));
                    if (scItem != null && !scItem.TemplateID.ToString().Equals(MediaConstants.MediaFolderTemplateID))
                    {
                        if (Context.Job != null)
                        {
                            Context.Job.Status.Messages.Add(string.Format("Recycling item ", scItem.Paths.FullPath));
                            Context.Job.Status.Processed++;
                        }

                        if (!scItem.HasChildren || recycleChildren)
                        {
                            scItem.RecycleChildren();
                            scItem.Recycle();
                        }
                    }
                }
            }

            this.UpdateStorageAfterCleanUp(itemIDs, ActionType.DeleteItem);
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
        /// <param name="itemIDs">A list of Sitecore item ID strings of the items to delete.</param>
        /// <param name="deleteChildren">If set to true, the underlying child items of each item will be deleted too.</param>
        public void Delete(List<string> itemIDs, bool deleteChildren)
        {
            if (Context.Job != null)
            {
                Context.Job.Status.Total = itemIDs.Count;
            }

            using (new SecurityDisabler())
            {
                foreach (var itemId in itemIDs)
                {
                    var scItem = this.database.GetItem(new ID(itemId));
                    if (scItem != null && !scItem.TemplateID.ToString().Equals(MediaConstants.MediaFolderTemplateID))
                    {
                        if (Context.Job != null)
                        {
                            Context.Job.Status.Messages.Add(string.Format("Deleting item ", scItem.Paths.FullPath));
                            Context.Job.Status.Processed++;
                        }

                        this.DeleteItem(scItem, deleteChildren);
                    }
                }
            }

            this.UpdateStorageAfterCleanUp(itemIDs, ActionType.DeleteItem);
        }

        /// <summary>
        /// Helper method used by Download and Delete methods of this class, to delete items child-aware.
        /// </summary>
        /// <remarks>
        /// Note that if deleteChildren is set to false, and the item does have children, the item will be changed to a folder only.
        /// The philosophy behind this is that it will still free up database space (which was the initial goal), but it won't break the item hierarchy.
        /// </remarks>
        /// <param name="item">The item to delete.</param>
        /// <param name="deleteChildren">If set to true, the children of the item will be deleted as well.</param>
        private void DeleteItem(Item item, bool deleteChildren)
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

        /// <summary>
        /// Changes a media item into a media folder by changing the template of the item.
        /// This orphans the media blob so we can clean it afterwards, without breaking the path to underlying items.
        /// </summary>
        /// <param name="item">The item to change to a folder.</param>
        private void ChangeToFolder(Item item)
        {
            var mediaFolderGuid = new ID(MediaConstants.MediaFolderTemplateID);
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
        /// <param name="itemIDs">A list of Sitecore item ID strings of the items to delete the old versions of.</param>
        public void DeleteOldVersions(List<string> itemIDs)
        {
            if (Context.Job != null)
            {
                Context.Job.Status.Total = itemIDs.Count;
            }

            using (new SecurityDisabler())
            {
                foreach (var itemId in itemIDs)
                {
                    var scItem = this.database.GetItem(new ID(itemId));
                    if (scItem != null && !scItem.TemplateID.ToString().Equals(MediaConstants.MediaFolderTemplateID))
                    {
                        if (Context.Job != null)
                        {
                            Context.Job.Status.Messages.Add(string.Format("Deleting old versions of item ", scItem.Paths.FullPath));
                            Context.Job.Status.Processed++;
                        }

                        this.DeleteOldVersions(scItem);
                    }
                }
            }

            this.UpdateStorageAfterCleanUp(itemIDs, ActionType.DeleteVersions);
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

        /// <summary>
        /// To avoid completely re-scanning the whole media library after a change, we could simply update the current JSON storage
        /// with the items that were cleaned up by our module. But take in mind that external changes are still being ignored by this module.
        /// So for an accurate rendition of the media library data usage, a re-scan is necessary once in a while.
        /// </summary>
        /// <remarks>
        /// The backlog of this module contains an idea to create a pipeline processor for the onSave handler of all items,
        /// so the JSON storage could be updated continuously, never requiring a re-scan after the initial media library scan!
        /// </remarks>
        /// <param name="itemIDs">A list of Sitecore item ID strings of the items to update in the JSON storage.</param>
        /// <param name="actionType">An ActionType enum object to indicate whether to delete the items in the list, or to remove the old versions from them.</param>
        private void UpdateStorageAfterCleanUp(List<string> itemIDs, ActionType actionType)
        {
            MediaItemReport mediaItemRoot = null;
            JsonStorage mediaItemReportStorage = null;

            // read the media item report object from the JSON storage
            var mediaItemPath = Settings.GetSetting("Shrink.MediaItemReportPath");
            if (!string.IsNullOrEmpty(mediaItemPath))
            {
                mediaItemReportStorage = new JsonStorage(mediaItemPath);
                mediaItemRoot = mediaItemReportStorage.Deserialize<MediaItemReport>();
            }

            if (mediaItemRoot != null)
            {
                // remove any children as supplied in the list of Sitecore item IDs
                this.RemoveChildren(mediaItemRoot, itemIDs, actionType);

                // write the updated JSON file to disk
                mediaItemReportStorage.Serialize(mediaItemRoot);

                // update the corresponding media library report JSON storage file with the updated info
                var libraryReportPath = Settings.GetSetting("Shrink.MediaLibraryReportPath");
                if (!string.IsNullOrEmpty(libraryReportPath))
                {
                    var json = new JsonStorage(libraryReportPath);
                    json.Serialize(new MediaLibraryReport(mediaItemRoot));
                }
            }
        }

        /// <summary>
        /// Recursively deletes items from the media item report object that are present in the given list of item IDs.
        /// </summary>
        /// <param name="mediaItemReport">The media item report object to delete the children of.</param>
        /// <param name="itemIDs">A list of Sitecore item ID strings of the items to remove.</param>
        private void RemoveChildren(MediaItemReport mediaItemReport, List<string> itemIDs, ActionType actionType)
        {
            var itemsToDelete = new List<MediaItemReport>();
            foreach (var item in mediaItemReport.Children)
            {
                // collect item references to delete
                var sitecoreItemIdString = string.Format("{{{0}}}", item.ID.ToString().ToUpper());
                if(item.IsMediaFolder.HasValue && !item.IsMediaFolder.Value && itemIDs.Contains(sitecoreItemIdString))
                {
                    // either stage the item for removal or just mark the old versions flag false, depending on the action type
                    if(actionType == ActionType.DeleteItem)
                    {
                        itemsToDelete.Add(item);
                    }
                    else if(actionType == ActionType.DeleteVersions)
                    {
                        item.HasOldVersions = false;
                    }
                }

                // check out the children of this item (recursively)
                if(item.Children != null)
                {
                    this.RemoveChildren(item, itemIDs, actionType);
                }
            }

            // remove the items to delete from the children object list
            foreach(var item in itemsToDelete)
            {
                mediaItemReport.Children.Remove(item);
            }
        }
    }
}