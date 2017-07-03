namespace robhabraken.SitecoreShrink.Controllers
{
    using Entities;
    using Helpers;
    using IO;
    using Sitecore.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
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

            var jobs = JobInfo.MyJobs;
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

        /// <summary>
        /// Returns a subset of the total media library based on the given category (see MediaConstants class for all categories)
        /// </summary>
        /// <param name="category">The category to get the items off.</param>
        /// <returns>A pipe separated list of Sitecore IDs representing all items that fall within the given category.</returns>
        public ActionResult SelectSubset(string category)
        {
            var subset = string.Empty;

            // reload the JSON report file to reconstruct the media item report
            var mediaItemPath = Settings.GetSetting("Shrink.MediaItemReportPath");
            if (!string.IsNullOrEmpty(mediaItemPath))
            {
                var json = new JsonStorage(mediaItemPath);
                var mediaItemRoot = json.Deserialize<MediaItemReport>();
                var libraryReport = new MediaLibraryReport(mediaItemRoot);
                
                List<MediaItemReport> items = null;

                switch(category)
                {
                    case MediaConstants.CategoryInUse:
                        items = libraryReport.ItemsByReferenceState(true);
                        break;
                    case MediaConstants.CategoryNotReferenced:
                        items = libraryReport.ItemsByReferenceState(false);
                        break;
                    case MediaConstants.CategoryReferencedUnknown:
                        items = libraryReport.ItemsReferencesUnknown();
                        break;
                    case MediaConstants.CategoryPublished:
                        items = libraryReport.ItemsByPublishingState(true);
                        break;
                    case MediaConstants.CategoryUnpublished:
                        items = libraryReport.ItemsByPublishingState(false);
                        break;
                    case MediaConstants.CategoryPublishedUnknown:
                        items = libraryReport.ItemsPublishingStateUnknown();
                        break;
                    case MediaConstants.CategoryItemsWithOldVersions:
                        items = libraryReport.ItemsByVersionState(true);
                        break;
                    case MediaConstants.CategoryItemsUsingAllVersions:
                        items = libraryReport.ItemsByVersionState(false);
                        break;
                    case MediaConstants.CategoryVersionsUnknown:
                        items = libraryReport.ItemsVersionsUnknown();
                        break;
                }

                if (items != null)
                {
                    subset = ItemHelper.ItemListToPipedString(items);
                }
            }            

            return Json(subset, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Starts a TidyUp job to archive all media items in the given item list.
        /// </summary>
        /// <param name="itemList">A pipe-separated list of Sitecore IDs of the items to archive.</param>
        /// <returns>The boolean value true if the job has started.</returns>
        public ActionResult ArchiveMedia(string itemList)
        {
            new TidyJobManager().Archive(this.PipedStringToList(itemList), false);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Starts a TidyUp job to recycle all media items in the given item list.
        /// </summary>
        /// <param name="itemList">A pipe-separated list of Sitecore IDs of the items to recycle.</param>
        /// <returns>The boolean value true if the job has started.</returns>
        public ActionResult RecycleMedia(string itemList)
        {
            new TidyJobManager().Recycle(this.PipedStringToList(itemList), false);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Starts a TidyUp job to delete all media items in the given item list.
        /// </summary>
        /// <param name="itemList">A pipe-separated list of Sitecore IDs of the items to delete.</param>
        /// <returns>The boolean value true if the job has started.</returns>
        public ActionResult DeleteMedia(string itemList)
        {
            new TidyJobManager().Delete(this.PipedStringToList(itemList), false);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Starts a TidyUp job to download all media items in the given item list.
        /// </summary>
        /// <param name="itemList">A pipe-separated list of Sitecore IDs of the items to download.</param>
        /// <param name="deleteAfterwards">If true, the media items will be deleted directly after downloading them.</param>
        /// <returns>The boolean value true if the job has started.</returns>
        public ActionResult DownloadMedia(string itemList, bool deleteAfterwards)
        {
            var downloadPath = Settings.GetSetting("Shrink.DownloadPath");
            //first, we need to check, if our path is absolute or relative
            //bring up downloadpath to one path separator
            downloadPath = downloadPath.Contains("\\") ? downloadPath.Replace('/', '\\') : downloadPath;
            //check if we can get full path via Path - if it is, then we have absolute path, else - it was relative
            downloadPath = Path.GetFullPath(downloadPath) == downloadPath ? downloadPath : $"{HttpRuntime.AppDomainAppPath}{downloadPath}";

            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            new TidyJobManager().Download(this.PipedStringToList(itemList), downloadPath, deleteAfterwards);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Starts a TidyUp job to delete the old versions of all media items in the given item list.
        /// </summary>
        /// <param name="itemList">A pipe-separated list of Sitecore IDs of the items to delete the old versions of.</param>
        /// <returns>The boolean value true if the job has started.</returns>
        public ActionResult DeleteOldVersions(string itemList)
        {
            new TidyJobManager().DeleteOldVersions(this.PipedStringToList(itemList));

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Converts a pipe-separated string to a string object list.
        /// </summary>
        /// <param name="pipedString">A pipe-separated string.</param>
        /// <returns>A List of string objects.</returns>
        private List<string> PipedStringToList(string pipedString)
        {
            return pipedString.Split('|').ToList();
        }
    }
}