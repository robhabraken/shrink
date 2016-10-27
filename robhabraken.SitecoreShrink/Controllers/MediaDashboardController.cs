namespace robhabraken.SitecoreShrink.Controllers
{
    using Sitecore.Data;
    using System;
    using System.Web.Mvc;

    public class MediaDashboardController : Controller
    {
        public ActionResult ScanMedia(string name)
        {
            name += " Scanning!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ZoomIn(Guid id)
        {
            // mock code
            var itemName = string.Empty;
            if (ID.IsID(id.ToString()))
            {
                var db = Sitecore.Configuration.Factory.GetDatabase("master");
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

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteOldVersions(string name)
        {
            name += " Deleting old versions!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }
    }
}