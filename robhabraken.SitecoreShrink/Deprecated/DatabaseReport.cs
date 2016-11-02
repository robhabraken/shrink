
namespace robhabraken.SitecoreShrink.Deprecated
{
    /// <summary>
    /// Database report holding all the different outcomes of the analyses.
    /// </summary>
    /// <remarks>
    /// PLEASE NOTE that this class isn't used anymore. It works fine on itself, but I did shift the main focus of this module towards analyzing and cleaning data within Sitecore itself.
    /// Next to that, cleaning up orphans is already possible in Sitecore (via the Control Panel) and this and other features of this class also require very specific user rights that
    /// may not be available on a regular Content Management server or its database user. Lastly, the beneath methods take very long to execute on a large database, so they are better 
    /// to be executed by a system engineer than via a Sitecore module.
    /// 
    /// However, for the time being I did not yet delete these classes, but they are probably going to disappear from this module completely in the near future.
    /// </remarks>
    public class DatabaseReport
    {
        public string DatabaseName { get; set; }
        public string DatabaseSize { get; set; }
        public string UnallocatedSpace { get; set; }

        public string Reserved { get; set; }
        public string Data { get; set; }
        public string IndexSize { get; set; }
        public string UnusedData { get; set; }
        
        public long UsedBlobsSizeInBytes { get; set; }
        public long UnusedBlobsSizeInBytes { get; set; }
    }
}
