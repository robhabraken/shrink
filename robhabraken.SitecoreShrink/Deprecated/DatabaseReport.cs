
namespace robhabraken.SitecoreShrink.Deprecated
{
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
