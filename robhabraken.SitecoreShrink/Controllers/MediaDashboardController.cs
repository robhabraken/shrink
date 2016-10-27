namespace robhabraken.SitecoreShrink.Controllers
{
    using System.Web.Mvc;

    public class MediaDashboardController : Controller
    {
        public ActionResult ScanMedia(string name)
        {
            name += " Scanning!";

            return Json(name, JsonRequestBehavior.AllowGet);
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

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteOldVersions(string name)
        {
            name += " Deleting old versions!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }
    }
}