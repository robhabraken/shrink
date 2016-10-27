namespace robhabraken.SitecoreShrink.Controllers
{
    using System.Web.Mvc;

    public class PropertyController : Controller
    {
        public ActionResult GetProperty(string name)
        {
            name += " Habraken!";

            return Json(name, JsonRequestBehavior.AllowGet);
        }
    }
}
