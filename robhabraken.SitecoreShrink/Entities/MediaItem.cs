namespace robhabraken.SitecoreShrink.Entities
{
    using System;
    using System.Collections.Generic;

    public class MediaItem
    {
        public MediaItem()
        {
            this.ID = Guid.Empty;
            this.Name = string.Empty;
            this.Children = new List<MediaItem>();
            this.Size = 0;
            this.IsMediaFolder = null;
            this.IsReferenced = null;
            this.IsPublished = null;
            this.HasOldVersions = null;
        }

        public Guid ID { get; set; }

        public string Name { get; set; }

        public List<MediaItem> Children { get; set; }

        /// <summary>
        /// Only applicable for media items, defaults to 0 when IsMediaFolder is true.
        /// </summary>
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
