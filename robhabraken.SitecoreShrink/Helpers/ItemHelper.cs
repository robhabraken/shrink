namespace robhabraken.SitecoreShrink.Helpers
{
    using Entities;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;

    /// <summary>
    /// Helper class with some generic Sitecore Item magic.
    /// </summary>
    public class ItemHelper
    {
        /// <summary>
        /// Gets a Sitecore Item object by its Guid, using the configured database used for scanning.
        /// </summary>
        /// <param name="id">The ID of the Item to retrieve.</param>
        /// <returns>A Sitecore Item object or null if the item is not found.</returns>
        public static Item GetItem(Guid id)
        {
            Item item = null;
            if (ID.IsID(id.ToString()))
            {
                var databaseName = Settings.GetSetting("Shrink.DatabaseToScan");
                var database = Factory.GetDatabase(databaseName);
                item = database.GetItem(new ID(id));
            }
            return item;
        }

        /// <summary>
        /// Returns the item path to the given item represented in Sitecore IDs,
        /// which is needed for the default SPEAK TreeView component, to be able to preload a certain path.
        /// </summary>
        /// <param name="item">The item to get the item path of.</param>
        /// <returns>A string representation of the ID item path, of which the item IDs are separated by slashes.</returns>
        public static string GetItemPathInGuids(Item item)
        {
            var path = item.ID.ToString();
            while(!item.ParentID.IsNull)
            {
                path = string.Format("{0}/{1}", item.ParentID.ToString(), path);

                if(item.ParentID.ToString().Equals(MediaConstants.MediaLibraryItemID))
                {
                    break;
                }
                else
                {
                    item = item.Parent;
                }
            }
            return path;
        }
    }
}
