namespace robhabraken.SitecoreShrink.Entities
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Helper class to generate the JSON data source for the donut charts.
    /// One chart stats object represents one donut chart, containing one or more children to represent the corresponding slices.
    /// </summary>
    [DataContract]
    public class ChartStats
    {
        [DataMember(Name = "children", Order = 2)]
        public List<ReportCategory> Children { get; set; }
    }
}
