namespace robhabraken.SitecoreShrink.Controllers
{
    using Helpers;
    using Sitecore.Data;
    using System;
    using System.Web.Mvc;
    using Tasks;

    public class MediaDashboardController : Controller
    {
        /// <summary>
        /// If a job of the Sitecore Shrink module is currently running, this method returns the friendly job description of that job.
        /// When no jobs are running, an empty string is returned.
        /// </summary>
        /// <returns>The friendly name of the active Shrink job, or an empty string when it is not running.</returns>
        public ActionResult GetActiveJobDescription()
        {
            var activeJobName = string.Empty;

            var jobs = JobInfo.Jobs;
            if(jobs.Count > 0)
            {
                activeJobName =  JobInfo.GetFriendlyJobName(jobs[0]);
            }

            return Json(activeJobName, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScanMedia()
        {
            var mediaScanner = new AnalyzeJobManager();
            mediaScanner.ScanMediaLibrary();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ZoomIn(Guid id)
        {
            // mock code
            var itemName = string.Empty;
            if (ID.IsID(id.ToString()))
            {
                var db = Sitecore.Configuration.Factory.GetDatabase("master"); // mock code!
                var item = db.GetItem(new ID(id));
                itemName = item.Name;
            }

            return Json(itemName, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectSubset(string selection)
        {
            selection = "Will select " + selection;

            return Json(selection, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ArchiveMedia(string name)
        {
            name += " Archiving!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecycleMedia(string name)
        {
            name += " Recycling!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMedia(string name)
        {
            name += " Deleting!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadMedia(string name)
        {
            name += " Downloading!";

            var downloadPath = ConfigurationHelper.ReadSetting("Shrink.DownloadPath");

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteOldVersions(string name)
        {
            name += " Deleting old versions!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }
    }
}