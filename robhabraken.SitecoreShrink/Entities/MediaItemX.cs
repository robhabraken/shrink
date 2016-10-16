namespace robhabraken.SitecoreShrink.Entities
{
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Analyzing;

    [DataContract]
    public class MediaItemX
    {
        public const string MEDIA_FOLDER_TEMPLATE_ID = "{FE5DD826-48C6-436D-B87A-7C4210C7413B}";

        public MediaItemX()
        {
            this.ID = Guid.Empty;
            this.Name = string.Empty;
            this.Children = new List<MediaItemX>();
            this.Size = 0;
            this.IsMediaFolder = null;
            this.IsReferenced = null;
            this.IsPublished = null;
            this.HasOldVersions = null;
        }

        public MediaItemX(Item item)
        {
            this.ID = item.ID.Guid;
            this.Name = item.Name;
            this.Children = new List<MediaItemX>();

            this.IsMediaFolder = item.Template.ID.ToString().Equals(MediaItemX.MEDIA_FOLDER_TEMPLATE_ID);

            if (this.IsMediaFolder.HasValue && !this.IsMediaFolder.Value && item.Paths.IsMediaItem)
            {
                var mediaItem = (MediaItem)item;
                this.Size = mediaItem.Size;
            }
        }

        public Guid ID { get; set; }
        
        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name = "children", Order = 2)]
        public List<MediaItemX> Children { get; set; } = null;

        /// <summary>
        /// Only applicable for media items, defaults to 0 when IsMediaFolder is true.
        /// </summary>
        [DataMember(Name = "size", EmitDefaultValue = false, Order = 3)]
        public long Size { get; set; }
        
        public bool? IsMediaFolder { get; set; }

        public bool? IsReferenced { get; set; }

        /// <summary>
        /// True if this item is published to at least one of the configured publishing targets.
        /// </summary>
        public bool? IsPublished { get; set; }

        public bool? HasOldVersions { get; set; }
    }
}
