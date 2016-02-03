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

namespace shrink
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var mediaItemUsage = new MediaItemUsage("master");
            var itemReport = mediaItemUsage.ScanMediaLibrary();
            foreach (var item in itemReport.UnusedItems)
            {
                Response.Write(item.ID + " " + item.Name + "<br/>");
            }
            Response.Write("total: " + itemReport.MediaItemCount.ToString() + "<br />");
            Response.Write("unused: " + itemReport.UnusedItems.Count.ToString() + "<br />");
            Response.Write("unpublished: " + itemReport.UnpublishedItems.Count.ToString() + "<br />");
            Response.Write("old versions: " + itemReport.OldVersions.Count.ToString() + "<br /><br />");

            var tidyJM = new TidyJobManager("master");
            //tidyJM.Archive();
            //tidyJM.Delete();
            //tidyJM.DeleteOldVersions();
            //tidyJM.Download();
            //tidyJM.Recycle();

            var databaseHelper = new DatabaseHelper("master");
            var dbReport = new DatabaseReport();
            databaseHelper.GetSpaceUsed(ref dbReport);
            Response.Write("database name: " + dbReport.DatabaseName + "<br />");
            Response.Write("database size: " + dbReport.DatabaseSize + "<br />");
            Response.Write("unallocated space: " + dbReport.UnallocatedSpace + "<br />");
            Response.Write("reserved: " + dbReport.Reserved + "<br />");
            Response.Write("data: " + dbReport.Data + "<br />");
            Response.Write("index size: " + dbReport.IndexSize + "<br />");
            Response.Write("unused: " + dbReport.UnusedData + "<br /><br />");

            databaseHelper.GetOrphanedBlobsSize(ref dbReport);
            Response.Write("used blobs: " + dbReport.UsedBlobs + " MB<br />");
            Response.Write("unused blobs: " + dbReport.UnusedBlobs + " MB<br /><br />");

            //databaseHelper.CleanUpOrphanedBlobs();
            //databaseHelper.ShrinkDatabase();
        }
    }
}