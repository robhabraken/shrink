namespace robhabraken.SitecoreShrink.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reporting class representing (a part of) the media library, containing metrics about its item usage.
    /// </summary>
    [DataContract]
    public class MediaLibraryReport
    {
        private IEnumerable<MediaItemReport> flatList;

        /// <summary>
        /// Constructs a media library report object based on (a part of) the media library tree, represented by a media item report object.
        /// </summary>
        /// <param name="mediaItemReport">The media item report object to generate a media library report for.</param>
        public MediaLibraryReport(MediaItemReport mediaItemReport)
        {
            this.flatList = mediaItemReport.Children.Flatten(x => x.Children);
            this.GenerateStats();
        }

        /// <summary>
        /// Returns the total number of media items in the media library, not including media folders.
        /// </summary>
        /// <returns>The total number of media items in the media library.</returns>
        public int MediaItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value);
        }

        /// <summary>
        /// Returns the total size of the media library in bytes, not including media folders.
        /// </summary>
        /// <returns>The total size of the media library in bytes.</returns>
        public long MediaLibrarySize()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns the number of media items that are referenced by other Sitecore items, not including media folders.
        /// </summary>
        /// <returns>The number of media items that are referenced by other Sitecore items.</returns>
        public int ReferencedItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && x.IsReferenced.Value);
        }

        /// <summary>
        /// Returns the total size of all referenced items in bytes, not including the media folders.
        /// </summary>
        /// <returns>The total size of all referenced items in bytes.</returns>
        public long ReferencedMediaSize()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && x.IsReferenced.Value).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns a list of all items that are not referenced, not including media folders.
        /// </summary>
        /// <returns>A list of all items that are not referenced.</returns>
        public List<MediaItemReport> UnreferencedItems()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && !x.IsReferenced.Value).ToList<MediaItemReport>();
        }

        /// <summary>
        /// Returns the number of media items that are published to one or more publishing targets, not including media folders.
        /// </summary>
        /// <returns>The number of media items that are published to one or more publishing targets.</returns>
        public int PublishedItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && x.IsPublished.Value);
        }

        /// <summary>
        /// Returns the total size of all published items in bytes (to one or more publishing targets), not including media folders.
        /// </summary>
        /// <returns>The total size of all published items in bytes.</returns>
        public long PublishedMediaSize()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && x.IsPublished.Value).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns a list of all items that are not published, not including media folders.
        /// </summary>
        /// <returns>A list of all items that are not published.</returns>
        public List<MediaItemReport> UnpublishedItems()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && !x.IsPublished.Value).ToList<MediaItemReport>();
        }

        /// <summary>
        /// Returns the number of media items that contain more than one version, not including media folders.
        /// </summary>
        /// <returns>The number of media items that contain more than one version.</returns>
        public int OldVersionsItemCount()
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.HasOldVersions.HasValue && x.HasOldVersions.Value);
        }

        /// <summary>
        /// Returns a list of all items that contain more than one version, not including media folders.
        /// </summary>
        /// <returns>A list of all items that contain more than one version.</returns>
        public List<MediaItemReport> ItemsWithOldVersions()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.HasOldVersions.HasValue && x.HasOldVersions.Value).ToList<MediaItemReport>();
        }

        /// <summary>
        /// Stores statistics generated at construction time, also used to generate the JSON data source for the donut charts.
        /// </summary>
        [DataMember(Name = "stats")]
        public List<ChartStats> Stats { get; set; }

        /// <summary>
        /// Generates the statistics object required to store the above stats in a JSON format to be used for the donut charts.
        /// </summary>
        private void GenerateStats()
        {
            this.Stats = new List<ChartStats>();

            var totalCount = this.MediaItemCount();
            var totalSize = this.MediaLibrarySize();
            var referencedItems = this.ReferencedMediaSize();
            var publishedItems = this.PublishedMediaSize();
            var itemsOldVersions = this.OldVersionsItemCount();

            this.Stats.Add(new ChartStats()
            {
                Children = new List<ReportCategory>() {
                    new ReportCategory(MediaConstants.CategoryInUse, referencedItems),
                    new ReportCategory(MediaConstants.CategoryNotReferenced, totalSize - referencedItems)
                }
            });

            this.Stats.Add(new ChartStats()
            {
                Children = new List<ReportCategory>() {
                    new ReportCategory(MediaConstants.CategoryPublished, publishedItems),
                    new ReportCategory(MediaConstants.CategoryUnpublished, totalSize - publishedItems)
                }
            });

            this.Stats.Add(new ChartStats()
            {
                Children = new List<ReportCategory>() {
                    new ReportCategory(MediaConstants.CategoryItemsWithOldVersions, itemsOldVersions),
                    new ReportCategory(MediaConstants.CategoryItemsUsingAllVersions, totalCount - itemsOldVersions)
                }
            });
        }
    }
}
