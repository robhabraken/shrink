<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GlobalHeader.ascx.cs" Inherits="Sitecore.ExperienceExplorer.Web.Web.ExperienceExplorer.Controls.GlobalHeader" %>
<%@ Import Namespace="System.Web.Helpers" %>
<%@ Import Namespace="Sitecore.Security.Accounts" %>

<link type="text/css" rel="stylesheet" href="/sitecore/shell/Themes/Standard/Default/GlobalHeader.css" />
<link type="text/css" rel="stylesheet" href="/sitecore modules/Web/ExperienceExplorer/Assets/css/experience-explorer-global-header.css" />
<%
  string controlSource;
  using (new UserSwitcher(User.FromName(Sitecore.ExperienceExplorer.Business.Managers.ModuleManager.RealUserName, true)))
  {
    controlSource = AntiForgery.GetHtml().ToString();
  }
%>
<% = controlSource %>
<header class="sc-globalHeader">
  <div class="sc-globalHeader-content">
    <div class="col2">
      <div class="sc-globalHeader-startButton">
        <a class="sc-global-logo" href="#" onclick='javascript:$.get("/?sc_mode=edit", function (){window.location="/sitecore/shell/sitecore/client/Applications/LaunchPad";});'></a>
      </div>
    </div>
    <div class="col2">
      <div class="sc-globalHeader-loginInfo">

        <ul class="sc-accountInformation">
          <li>
            <span class="logout" onclick='javascript:$.post("/api/sitecore/Authentication/Logout?sc_database=master", $("[name=__RequestVerificationToken]"), function (){window.location.reload();});'>
              <%=GetLogoutHeaderText()%>
            </span>
          </li>
          <li>
            <%=GetGlobalHeaderUserName()%>
            <img src="<%=GetGlobalHeaderUserPortraitUrl()%>" />
          </li>
        </ul>
      </div>
    </div>
  </div>
</header>
