﻿namespace robhabraken.SitecoreShrink.Controllers
{
    using Helpers;
    using Sitecore.Configuration;
    using System;
    using System.Collections.Generic;
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
            var activeJobInfo = new List<object>();

            var jobs = JobInfo.Jobs;
            if(jobs.Count > 0)
            {
                activeJobInfo =  JobInfo.GetJobInfo(jobs[0]);
            }

            return Json(activeJobInfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Starts the scanning of the media library as a Sitecore job.
        /// </summary>
        /// <returns>The boolean value true if the scanning has started.</returns>
        public ActionResult ScanMedia()
        {
            var mediaScanner = new AnalyzeJobManager();
            mediaScanner.ScanMediaLibrary();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns the string Item path in Guids to the selected item.
        /// This is used by the sunburst chart to send the path to select to the treeview component.
        /// </summary>
        /// <param name="id">The ID of the Item to select.</param>
        /// <returns>A string representation of the ID item path, of which the item IDs are separated by slashes.</returns>
        public ActionResult ZoomIn(Guid id)
        {
            var itemPath = string.Empty;
            var item = ItemHelper.GetItem(id);
            if (item != null)
            {
                itemPath = ItemHelper.GetItemPathInGuids(item);
            }

            return Json(itemPath, JsonRequestBehavior.AllowGet);
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

            var downloadPath = Settings.GetSetting("Shrink.DownloadPath");

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteOldVersions(string name)
        {
            name += " Deleting old versions!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }
    }
}