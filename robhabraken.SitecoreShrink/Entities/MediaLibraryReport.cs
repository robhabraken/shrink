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
        /// Returns the total size of all referenced or unreferenced items in bytes (depending on the boolean parameter), not including the media folders.
        /// </summary>
        /// <param name="referenced">A boolean indicating whether to select the referenced or the unreferenced items.</param>
        /// <returns>The total size of all referenced or unreferenced items in bytes, depending on the input parameter of this method.</returns>
        public long MediaSizeByReferenceState(bool referenced)
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && x.IsReferenced.Value == referenced).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns a list of all items that are referenced or not (depending on the boolean parameter), not including media folders.
        /// </summary>
        /// <param name="referenced">A boolean indicating whether to select the referenced or the unreferenced items.</param>
        /// <returns>A list of all items that are either referenced or not referenced, depending on the input parameter of this method.</returns>
        public List<MediaItemReport> ItemsByReferenceState(bool referenced)
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsReferenced.HasValue && x.IsReferenced.Value == referenced).ToList<MediaItemReport>();
        }

        /// <summary>
        /// Returns a list of all items where the references could not be determined.
        /// </summary>
        /// <returns>A list of all items of which the references could not be determined.</returns>
        public List<MediaItemReport> ItemsReferencesUnknown()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && !x.IsReferenced.HasValue).ToList<MediaItemReport>();
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
        /// Returns the total size of all published or unpublished items (depending on the boolean parameter) in bytes, not including media folders.
        /// </summary>
        /// <param name="published">A boolean indicating whether to select the published or the unpublished items.</param>
        /// <returns>The total size of all published or unpublished items in bytes, depending on the input parameter of this method.</returns>
        public long MediaSizeByPublishingState(bool published)
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && x.IsPublished.Value == published).Sum(x => x.Size);
        }

        /// <summary>
        /// Returns a list of all items that are published or not (depending on the boolean parameter), not including media folders.
        /// </summary>
        /// <param name="published">A boolean indicating whether to select the published or the unpublished items.</param>
        /// <returns>A list of all items that are either published or not published, depending on the input parameter of this method.</returns>
        public List<MediaItemReport> ItemsByPublishingState(bool published)
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && x.IsPublished.Value == published).ToList<MediaItemReport>();
        }

        /// <summary>
        /// Returns a list of all items where the publishing status could not be determined.
        /// </summary>
        /// <returns>A list of all items of which the publishing status could not be determined.</returns>
        public List<MediaItemReport> ItemsPublishingStateUnknown()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.IsPublished.HasValue && !x.IsPublished.Value).ToList<MediaItemReport>();
        }

        /// <summary>
        /// Returns the number of media items that contain more than one version or that use all versions (depending on the boolean parameter), not including media folders.
        /// </summary>
        /// <param name="hasOldVersions">A boolean indicating whether to select the items with old versions or the items that use all versions.</param>
        /// <returns>The number of media items that contain either old versions or not, depending on the input parameter of this method.</returns>
        public int ItemCountByVersionState(bool hasOldVersions)
        {
            return flatList.Count(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.HasOldVersions.HasValue && x.HasOldVersions.Value == hasOldVersions);
        }

        /// <summary>
        /// Returns a list of all items that contain more than one version or not (depending on the boolean parameter), not including media folders.
        /// </summary>
        /// <param name="hasOldVersions">A boolean indicating whether to select the items with old versions or the items that use all versions.</param>
        /// <returns>A list of all items that contain either old versions or not, depending on the input parameter of this method.</returns>
        public List<MediaItemReport> ItemsByVersionState(bool hasOldVersions)
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && x.HasOldVersions.HasValue && x.HasOldVersions.Value == hasOldVersions).ToList<MediaItemReport>();
        }

        /// <summary>
        /// Returns a list of all items where the version status could not be determined.
        /// </summary>
        /// <returns>A list of all items of which the version status could not be determined.</returns>
        public List<MediaItemReport> ItemsVersionsUnknown()
        {
            return flatList.Where(x => x.IsMediaFolder.HasValue && !x.IsMediaFolder.Value && !x.HasOldVersions.HasValue).ToList<MediaItemReport>();
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
            var referencedItems = this.MediaSizeByReferenceState(true);
            var unreferencedItems = this.MediaSizeByReferenceState(false);
            var publishedItems = this.MediaSizeByPublishingState(true);
            var unpublishedItems = this.MediaSizeByPublishingState(false);
            var itemsOldVersions = this.ItemCountByVersionState(true);
            var usingAllVersions = this.ItemCountByVersionState(false);

            this.Stats.Add(new ChartStats()
            {
                Children = new List<ReportCategory>() {
                    new ReportCategory(MediaConstants.CategoryInUse, referencedItems),
                    new ReportCategory(MediaConstants.CategoryNotReferenced, unreferencedItems),
                    new ReportCategory(MediaConstants.CategoryReferencedUnknown, totalSize - referencedItems - unreferencedItems)
                }
            });

            this.Stats.Add(new ChartStats()
            {
                Children = new List<ReportCategory>() {
                    new ReportCategory(MediaConstants.CategoryPublished, publishedItems),
                    new ReportCategory(MediaConstants.CategoryUnpublished, unpublishedItems),
                    new ReportCategory(MediaConstants.CategoryPublishedUnknown, totalSize - publishedItems - unpublishedItems)
                }
            });

            this.Stats.Add(new ChartStats()
            {
                Children = new List<ReportCategory>() {
                    new ReportCategory(MediaConstants.CategoryItemsWithOldVersions, itemsOldVersions),
                    new ReportCategory(MediaConstants.CategoryItemsUsingAllVersions, usingAllVersions),
                    new ReportCategory(MediaConstants.CategoryVersionsUnknown, totalCount - itemsOldVersions - usingAllVersions)
                }
            });
        }
    }
}
