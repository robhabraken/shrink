<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpHeader.ascx.cs" Inherits="Sitecore.ExperienceExplorer.Web.Controls.ExpHeader" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business.Helpers" %>
<link rel="stylesheet" type="text/css" href="/sitecore/shell/client/Speak/Assets/css/speak-default-theme.css" />
<link rel="stylesheet" type="text/css" href="/sitecore modules/Web/ExperienceExplorer/Assets/vendors/jquery-ui/css/jquery-ui-1.9.1.custom.css" />
<link rel="stylesheet" type="text/css" href="/sitecore modules/Web/ExperienceExplorer/Assets/css/experience-explorer-iframe.css" />
<script src="/sitecore modules/web/experienceExplorer/assets/vendors.min.js" type="text/javascript"></script>
<% var mapProvider = GeoIpHelpers.GetGeoIpProvider();
   if (mapProvider != null && !string.IsNullOrEmpty(mapProvider.Scripts))
   {
     Response.Write(mapProvider.Scripts);
   } %>
