namespace robhabraken.SitecoreShrink.Analyzing
{
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public static class ExtensionMethods
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursion)
        {
            return source.SelectMany(x => recursion(x).Flatten(recursion)).Concat(source);
        }
    }

    public class MediaReport
    {
        private IEnumerable<MediaItemX> flatList;

        public MediaReport(MediaItemX mediaItemX)
        {
            this.flatList = mediaItemX.Children.Flatten(x => x.Children);
        }

        /// <summary>
        /// Returns the total number of media items in the media library, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int MediaItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value);
        }

        /// <summary>
        /// Returns the total size of the media library in bytes, not including media folders.
        /// </summary>
        /// <returns></returns>
        public long MediaLibrarySize()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns the number of media items that are referenced by other Sitecore items, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int ReferencedItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && x.IsReferenced.Value);
        }

        /// <summary>
        /// Returns the total size of all referenced items in bytes, not including the media folders.
        /// </summary>
        /// <returns></returns>
        public long ReferencedMediaSize()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && x.IsReferenced.Value).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns a list of all items that are not referenced, not including media folders, for clean up purposes.
        /// </summary>
        /// <returns></returns>
        public List<MediaItemX> UnreferencedItems()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && !x.IsReferenced.Value).ToList<MediaItemX>();
        }

        /// <summary>
        /// Returns the number of media items that are published to one or more publishing targets, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int PublishedItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && x.IsPublished.Value);
        }

        /// <summary>
        /// Returns the total size of all published items in bytes (to one or more publishing targets), not including media folders.
        /// </summary>
        /// <returns></returns>
        public long PublishedMediaSize()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && x.IsPublished.Value).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns a list of all items that are not published, not including media folders, for clean up purposes.
        /// </summary>
        /// <returns></returns>
        public List<MediaItemX> UnpublishedItems()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && !x.IsPublished.Value).ToList<MediaItemX>();
        }
        
        /// <summary>
        /// Returns the number of media items that contain more than one version, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int OldVersionsItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.HasOldVersions.HasValue && x.HasOldVersions.Value);
        }

        /// <summary>
        /// Returns a list of all items that contain old versions, not including media folders, for clean up purposes.
        /// </summary>
        /// <returns></returns>
        public List<MediaItemX> ItemsWithOldVersions()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.HasOldVersions.HasValue && x.HasOldVersions.Value).ToList<MediaItemX>();
        }
    }
}
