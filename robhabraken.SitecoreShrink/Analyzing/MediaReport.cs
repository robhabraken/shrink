namespace robhabraken.SitecoreShrink.Analyzing
{
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MediaReport
    {

        public List<MediaItemX> flatList;

        public MediaReport(MediaItemX mediaItemX)
        {

        }

        /// <summary>
        /// Returns the total number of media items in the media library, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int MediaItemCount()
        {
            return 0;
        }

        /// <summary>
        /// Returns the total size of the media library in bytes, not including media folders.
        /// </summary>
        /// <returns></returns>
        public long MediaLibrarySize()
        {
            return 0;
        }

        /// <summary>
        /// Returns the number of media items that are referenced by other Sitecore items, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int ReferencedItemCount()
        {
            return 0;
        }

        /// <summary>
        /// Returns the total size of all referenced items in bytes, not including the media folders.
        /// </summary>
        /// <returns></returns>
        public long ReferencedMediaSize()
        {
            return 0;
        }

        /// <summary>
        /// Returns the number of media items that are published to one or more publishing targets, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int PublishedItemCount()
        {
            return 0;
        }

        /// <summary>
        /// Returns the total size of all published items in bytes (to one or more publishing targets), not including media folders.
        /// </summary>
        /// <returns></returns>
        public int PublishedMediaSize()
        {
            return 0;
        }
        
        /// <summary>
        /// Returns the number of media items that contain more than one version, not including media folders.
        /// </summary>
        /// <returns></returns>
        public int OldVersionsItemCount()
        {
            return 0;
        }
    }
}
