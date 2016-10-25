namespace robhabraken.SitecoreShrink.Entities
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Helper class to generate the JSON data source for the donut charts.
    /// Each report category represents one slice within a donut chart.
    /// </summary>
    [DataContract]
    public class ReportCategory
    {
        public ReportCategory(string category, long size)
        {
            this.Category = category;
            this.Size = size;
        }

        [DataMember(Name = "category", Order = 1)]
        public string Category { get; set; }

        [DataMember(Name = "size", Order = 2)]
        public long Size { get; set; }
    }
}
