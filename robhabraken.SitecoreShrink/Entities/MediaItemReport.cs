namespace robhabraken.SitecoreShrink.Entities
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MediaItemReport
    {
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

        public MediaItemReport Concat(IEnumerable<MediaItemReport> moreChildren)
        {
            this.Children.AddRange(moreChildren);
            return this;
        }

        public Item GetSitecoreItem(string databaseName)
        {
            var database = Factory.GetDatabase(databaseName);
            return database != null ? database.Items[new ID(this.ID)] : null;
        }

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
        /// Only applicable for media items, defaults to 0 when IsMediaFolder is true.
        /// </summary>
        [DataMember(Name = "size", EmitDefaultValue = false, Order = 3)]
        public long Size { get; set; }

        [DataMember(Name = "mediafolder", EmitDefaultValue = false, Order = 5)]
        public bool? IsMediaFolder { get; set; }

        [DataMember(Name = "referenced", EmitDefaultValue = false, Order = 6)]
        public bool? IsReferenced { get; set; }

        /// <summary>
        /// True if this item is published to at least one of the configured publishing targets.
        /// </summary>
        [DataMember(Name = "published", EmitDefaultValue = false, Order = 7)]
        public bool? IsPublished { get; set; }

        [DataMember(Name = "oldversions", EmitDefaultValue = false, Order = 8)]
        public bool? HasOldVersions { get; set; }
    }
}
