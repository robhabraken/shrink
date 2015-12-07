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
            var exampleItem1 = Factory.GetDatabase("master").GetItem(new Sitecore.Data.ID("{04DAD0FD-DB66-4070-881F-17264CA257E1}"));
            var exampleItem2 = Factory.GetDatabase("master").GetItem(new Sitecore.Data.ID("{6AA5AA9F-071A-4808-91AC-709FAAFFB310}"));
            var exampleItem3 = Factory.GetDatabase("master").GetItem(new Sitecore.Data.ID("{9775973B-91D8-4FEF-A130-A774C61CA4AE}"));
            var exampleItem4 = Factory.GetDatabase("master").GetItem(new Sitecore.Data.ID("{8E158CC2-5F5D-426E-8897-F9573EBA976E}"));
            var exampleItem5 = Factory.GetDatabase("master").GetItem(new Sitecore.Data.ID("{5FDB5F8B-4D4D-4B29-AD0B-4E4F7F2460CC}"));

            var mediaItemUsage = new MediaItemUsage("master");
            mediaItemUsage.Scan(); // this should be automated or should be handled internally
            foreach (var item in mediaItemUsage.UnusedItems)
            {
                Response.Write(item.ID + " " + item.Name + "<br/>");
            }

            var tidyUp = new TidyUp("master");
            //works: tidyUp.Download(new List<Item>() { exampleItem1, exampleItem2, exampleItem3 }, "D:/");
            //works: tidyUp.Archive(new List<Item>() { exampleItem4 }, false);

            //TODO: implement orphaned blobs method
        }
    }
}