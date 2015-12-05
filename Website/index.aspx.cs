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
            var exampleItem = Factory.GetDatabase("master").GetItem(new Sitecore.Data.ID("{04DAD0FD-DB66-4070-881F-17264CA257E1}"));

            var tidyUp = new TidyUp();
            tidyUp.Download(new List<Item>() { exampleItem }, "D:\\");
        }
    }
}