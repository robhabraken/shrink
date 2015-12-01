<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpFooter.ascx.cs" Inherits="Sitecore.ExperienceExplorer.Web.sitecore_modules.Web.ExperienceExplorer.Controls.ExpFooter" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business.Constants" %>

<!-- COMMON VIEWS -->
<script id="HeaderTmpl" type="text/x-jsrender">
    {{for itemData}}
        <div class="tab-header">
          {{if QuestionMark}}
              <a href="#" title="{{:QuestionMark}}" class="info" data-toggle="tooltip" data-placement="left">?</a>
          {{/if}}
          <legend class="tab-title">{{:Title}}</legend>
        </div>

        {{if Description}}
            <span class="help-block tab-description">{{:Description}}</span>
        {{/if}}

    {{/for}}
</script>

<script src="/sitecore modules/Web/ExperienceExplorer/Assets/vendors.min.js" type="text/javascript"></script>
<script src="/sitecore modules/Web/ExperienceExplorer/Assets/experience-explorer-iframe.min.js" type="text/javascript"> </script>