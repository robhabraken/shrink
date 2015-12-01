<%@ Control Language="C#" Inherits="System.Web.UI.UserControl" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business.Constants" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business.Helpers" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business.Utilities" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Web" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business" %>

<link rel="stylesheet" type="text/css" href="/sitecore modules/Web/ExperienceExplorer/Assets/css/experience-explorer.css" />

<div class="experience-explorer">
  <div class="panel editor">
    <span id="pageEditorHeader"></span>
    <button type="button" class="page-editor-button" onclick="javascript:window.top.location.href='/?sc_mode=edit&sc_itemid=<%= Sitecore.Context.Item.ID.ToString() %>';"><%= ExperienceExplorerUtil.TranslateText(Texts.ExperienceEditor) %></button>
    <a class="trigger" href="#"></a>
    <iframe scrolling="no" src="<%= Paths.Module.Controls.RelativePath %>ExpEditor.aspx?<%=SettingsHelper.AddOnQueryStringKey%>=1&<%=SettingsHelper.ContextItemIdQueryStringKey %>=<%=Sitecore.Context.Item.ID.ToString() %>&<%=SettingsHelper.ContextDeviceIdQueryStringKey %>=<%=Sitecore.Context.Device.ID.ToString() %>&sc_pagesite=<%=Sitecore.Context.Site.Name %>&sc_preventProfilesScoring=1&sc_lang=<%= Sitecore.ExperienceExplorer.Web.Controls.ExpExplorerExtender.LanguageName %>" id="IframeExperienceExplorerEditor" class="ee-iframe"></iframe>
  </div>
  <div class="panel viewer">
    <a class="trigger" href="#"></a>
    <iframe scrolling="no" src="<%= Paths.Module.Controls.RelativePath %>ExpViewer.aspx?<%=SettingsHelper.AddOnQueryStringKey%>=1&<%=SettingsHelper.ContextItemIdQueryStringKey %>=<%=Sitecore.Context.Item.ID.ToString() %>&<%=SettingsHelper.ContextDeviceIdQueryStringKey %>=<%=Sitecore.Context.Device.ID.ToString() %>&sc_pagesite=<%=Sitecore.Context.Site.Name %>&sc_preventProfilesScoring=1&sc_lang=<%= Sitecore.ExperienceExplorer.Web.Controls.ExpExplorerExtender.LanguageName %>" id="IframeExperienceExplorerViewer" class="ee-iframe"></iframe>
  </div>
</div>

<script src="/sitecore modules/Web/ExperienceExplorer/Assets/experience-explorer.min.js"></script>
