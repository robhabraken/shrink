
namespace robhabraken.SitecoreShrink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DatabaseReport
    {
        public string DatabaseName { get; set; }
        public string DatabaseSize { get; set; }
        public string UnallocatedSpace { get; set; }

        public string Reserved { get; set; }
        public string Data { get; set; }
        public string IndexSize { get; set; }
        public string Unused { get; set; }
    }
}
