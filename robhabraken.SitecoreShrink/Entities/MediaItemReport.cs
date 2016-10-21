namespace robhabraken.SitecoreShrink.Entities
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a Sitecore media item, referencing the original item by ID and name, and containing meta data about its usage.
    /// 
    /// This item is a recursively nested object, containing subsequent lists of all children in the same tree structure as (parts of) the media library in Sitecore. 
    /// </summary>
    [DataContract]
    public class MediaItemReport
    {
        /// <summary>
        /// Constructs an empty media item report object using default values only.
        /// </summary>
        public MediaItemReport()
        {
            this.ID = Guid.Empty;
            this.Name = string.Empty;
            this.Children = new List<MediaItemReport>();
            this.Size = 0;
            this.IsMediaFolder = null;
            this.IsReferenced = null;
            this.IsPublished = null;
            this.HasOldVersions = null;
        }

        /// <summary>
        /// Constructs a media item report object from its corresponding Sitecore media item.
        /// 
        /// Mind that only the item properties are filled at construction time. The children list is initialized empty and usage analysis isn't done yet.
        /// </summary>
        /// <param name="item">A Sitecore media item to create an item report for.</param>
        public MediaItemReport(Item item)
        {
            this.ID = item.ID.Guid;
            this.Name = item.Name;
            this.Children = new List<MediaItemReport>();

            this.IsMediaFolder = item.Template.ID.ToString().Equals(MediaConstants.MediaFolderTemplateID);

            if (this.IsMediaFolder.HasValue && !this.IsMediaFolder.Value && item.Paths.IsMediaItem)
            {
                var mediaItem = (MediaItem)item;
                this.Size = mediaItem.Size;
            }
        }

        /// <summary>
        /// Concatenation method used by the extension method to flatten the recursive object tree. 
        /// </summary>
        /// <param name="moreChildren">A list of media item report objects that need to be added to the list of child items.</param>
        /// <returns>An update media item report object containing the children of both the source object as well as the supplied children.</returns>
        public MediaItemReport Concat(IEnumerable<MediaItemReport> moreChildren)
        {
            this.Children.AddRange(moreChildren);
            return this;
        }

        /// <summary>
        /// Returns the original Sitecore media item that this media item report object is referring to.
        /// 
        /// Mind that this Sitecore item isn't stored within this report (to improve performance and storage size,
        /// and to always get the latest version of the Sitecore item, even if the report is generated in an earlier point of time),
        /// but the item is retrieved from the given database on execution time.
        /// </summary>
        /// <param name="databaseName">The name of the database to get the corresponding Sitecore item from.</param>
        /// <returns>The original Sitecore media item that this report object is referring to.</returns>
        public Item GetSitecoreItem(string databaseName)
        {
            var database = Factory.GetDatabase(databaseName);
            return database != null ? database.Items[new ID(this.ID)] : null;
        }

        /// <summary>
        /// Returns the original Sitecore media item that this media item report object is referring to.
        /// 
        /// Mind that this Sitecore item isn't stored within this report (to improve performance and storage size,
        /// and to always get the latest version of the Sitecore item, even if the report is generated in an earlier point of time),
        /// but the item is retrieved from the given database on execution time.
        /// </summary>
        /// <param name="databaseName">The Sitecore database object to get the corresponding Sitecore item from.</param>
        /// <returns>The original Sitecore media item that this report object is referring to.</returns>
        public Item GetSitecoreItem(Database database)
        {
            return database != null ? database.Items[new ID(this.ID)] : null;
        }

        [DataMember(Name = "id", Order = 4)]
        public Guid ID { get; set; }
        
        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name = "children", Order = 2)]
        public List<MediaItemReport> Children { get; set; }

        /// <summary>
        /// The size in bytes of the corresponding media item.
        /// Only applicable for media items, defaults to 0 when IsMediaFolder is true.
        /// </summary>
        [DataMember(Name = "size", EmitDefaultValue = false, Order = 3)]
        public long Size { get; set; }

        [DataMember(Name = "mediafolder", EmitDefaultValue = false, Order = 5)]
        public bool? IsMediaFolder { get; set; }

        /// <summary>
        /// Indicates whether the media item is referenced by other Sitecore items in the same database, thus indicating if the item is being used or not.
        /// </summary>
        [DataMember(Name = "referenced", EmitDefaultValue = false, Order = 6)]
        public bool? IsReferenced { get; set; }

        /// <summary>
        /// True if this item is published to at least one of the configured publishing targets.
        /// </summary>
        [DataMember(Name = "published", EmitDefaultValue = false, Order = 7)]
        public bool? IsPublished { get; set; }

        /// <summary>
        /// True if the item contains more than one version, indicating one or more versions may be obsolete.
        /// </summary>
        [DataMember(Name = "oldversions", EmitDefaultValue = false, Order = 8)]
        public bool? HasOldVersions { get; set; }
    }
}
