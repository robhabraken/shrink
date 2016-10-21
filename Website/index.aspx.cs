using Sitecore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using robhabraken.SitecoreShrink;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using robhabraken.SitecoreShrink.Entities;
using System.Diagnostics;
using System.Threading;
using robhabraken.SitecoreShrink.IO;
using robhabraken.SitecoreShrink.Tasks;

namespace shrink
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var mediaScanner = new AnalyzeJobManager("master");
            mediaScanner.ScanMediaLibrary();
            Response.Write("Job started");

            //var stopwatch = Stopwatch.StartNew();
            //while (true)
            //{
            //    if (stopwatch.ElapsedMilliseconds > 180000)
            //    {
            //        mediaScanner.Stop();

            //        var stopwatch2 = Stopwatch.StartNew();
            //        while (true)
            //        {
            //            if (stopwatch2.ElapsedMilliseconds > 180000)
            //            {

            //                var json = new SomethingJSON();
            //                json.Serialize(mediaScanner.MediaItemRoot);

            //                return;
            //            }
            //            Thread.Sleep(500);
            //        }

            //    }
            //    Thread.Sleep(500);
            //}

            //var tidyJM = new TidyJobManager("master");
            //tidyJM.Download(itemReport.UnusedItems, @"D:\test\", false);
            //tidyJM.Archive();
            //tidyJM.Delete();
            //tidyJM.DeleteOldVersions();
            //tidyJM.Download();
            //tidyJM.Recycle();

            //var databaseHelper = new DatabaseHelper("master");
            //var dbReport = new DatabaseReport();
            //databaseHelper.GetSpaceUsed(ref dbReport);
            //Response.Write("database name: " + dbReport.DatabaseName + "<br />");
            //Response.Write("database size: " + dbReport.DatabaseSize + "<br />");
            //Response.Write("unallocated space: " + dbReport.UnallocatedSpace + "<br />");
            //Response.Write("reserved: " + dbReport.Reserved + "<br />");
            //Response.Write("data: " + dbReport.Data + "<br />");
            //Response.Write("index size: " + dbReport.IndexSize + "<br />");
            //Response.Write("unused: " + dbReport.UnusedData + "<br /><br />");

            //databaseHelper.GetOrphanedBlobsSize(ref dbReport);
            //Response.Write("used blobs: " + dbReport.UsedBlobs + " MB<br />");
            //Response.Write("unused blobs: " + dbReport.UnusedBlobs + " MB<br /><br />");

            //databaseHelper.CleanUpOrphanedBlobs();
            //databaseHelper.ShrinkDatabase();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var itemReport = new JsonStorage(@"D:\test.json").Deserialize();

            var stopwatch = Stopwatch.StartNew();

            var report = new MediaLibraryReport(itemReport);

            var a = report.MediaItemCount();
            var b = report.MediaLibrarySize();
            var c = report.ReferencedItemCount();
            var d = report.ReferencedMediaSize();
            var f = report.PublishedItemCount();
            var g = report.PublishedMediaSize();
            var h = report.OldVersionsItemCount();

            stopwatch.Stop();
            
        }
    }
}