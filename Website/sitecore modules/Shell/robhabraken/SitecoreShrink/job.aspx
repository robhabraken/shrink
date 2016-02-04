<%@ Page language="c#" EnableEventValidation="false" AutoEventWireup="true" %>
 
<script runat="server">
 
  void Page_Load(object sender, System.EventArgs e)
  {
    repJobs.DataBind();
  }
 
  public IEnumerable<Sitecore.Jobs.Job> Jobs
  {
    get
    {
      if (!cbShowFinished.Checked)
        return Sitecore.Jobs.JobManager.GetJobs().Where(job => job.IsDone == false).OrderBy(job => job.QueueTime);
      return Sitecore.Jobs.JobManager.GetJobs().OrderBy(job => job.QueueTime);
    }
  }
 
  protected string GetJobText(Sitecore.Jobs.Job job)
  {
    return string.Format("{0}\n\n{1}\n\n{2}", job.Name, job.Category, GetJobMessages(job));
  }
 
  protected string GetJobMessages(Sitecore.Jobs.Job job)
  {
    System.Text.StringBuilder sb = new StringBuilder();
    if (job.Options.ContextUser != null)
      sb.AppendLine("Context User: " + job.Options.ContextUser.Name);
    sb.AppendLine("Priority: " + job.Options.Priority.ToString());
    sb.AppendLine("Messages:");
    foreach (string s in job.Status.Messages)
      sb.AppendLine(s);
    return sb.ToString();
  }
 
  protected string GetJobColor(Sitecore.Jobs.Job job)
  {
    if (job.IsDone)
      return "#737373";
    return "#000";
  }
 
  protected void cbShowFinished_CheckedChanged(object sender, EventArgs e)
  {
    repJobs.DataBind();
  }
</script>  
 
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Job Viewer</title>
    <link href="/default.css" rel="stylesheet">
  </head>
  <body style="font-size:14px">
    <form runat="server">
 
      <div style="padding:10px; background-color:#efefef; border-bottom:solid 1px #aaa; border-top:solid 1px white">
        <div style="float:left; width:200px; padding-top:4px">
          <asp:CheckBox ID="cbShowFinished" runat="server" Text="Show finished jobs" Checked="false" OnCheckedChanged="cbShowFinished_CheckedChanged" AutoPostBack="true" />
        </div>
        <div style="float:right;">
          <asp:Button ID="btnRefresh" runat="server" Text="Refresh" BackColor="Green" ForeColor="White" Width="100px" Height="30px" />
        </div>
        <div style="clear:both;height:1px">&nbsp;</div>
      </div>
 
      <div style="padding-top:0px">
        <asp:Repeater ID="repJobs" runat="server" DataSource="<%# Jobs %>">
          <HeaderTemplate>
            <table style="width:100%">
              <thead style="background-color:#eaeaea">
                <td>Job</td>
                <td>Category</td>
                <td>Status</td>
                <td>Processed</td>
                <td>QueueTime</td>
              </thead>
          </HeaderTemplate>
          <FooterTemplate>
            </table>
          </FooterTemplate>
          <ItemTemplate>
            <tr style="background-color:beige; color:<%# GetJobColor((Container.DataItem as Sitecore.Jobs.Job)) %>" title="<%# GetJobText((Container.DataItem as Sitecore.Jobs.Job)) %>">
              <td>
                <%# Sitecore.StringUtil.Clip((Container.DataItem as Sitecore.Jobs.Job).Name, 50, true) %>
              </td>
              <td>
                <%# Sitecore.StringUtil.Clip((Container.DataItem as Sitecore.Jobs.Job).Category, 50, true) %>
              </td>
              <td>
                <%# (Container.DataItem as Sitecore.Jobs.Job).Status.State %>
              </td>
              <td>
                <%# (Container.DataItem as Sitecore.Jobs.Job).Status.Processed %> /
                <%# (Container.DataItem as Sitecore.Jobs.Job).Status.Total %>
              </td>
              <td>
                <%# (Container.DataItem as Sitecore.Jobs.Job).QueueTime.ToLocalTime() %>
              </td>
            </tr>
          </ItemTemplate>
        </asp:Repeater>
      </div>
 
    </form>
  </body>
</html>