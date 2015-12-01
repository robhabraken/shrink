<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpViewer.aspx.cs" Inherits="Sitecore.ExperienceExplorer.Web.Controls.ExpViewer1" %>

<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business.Entities" %>
<%@ Import Namespace="Sitecore.ExperienceExplorer.Business.Managers" %>
<%@ Import Namespace="Sitecore.Globalization" %>

<%@ Register Src="~/sitecore modules/Web/ExperienceExplorer/Controls/ExpHeader.ascx" TagPrefix="expExplorer" TagName="ExpHeader" %>
<%@ Register Src="~/sitecore modules/Web/ExperienceExplorer/Controls/ExpFooter.ascx" TagPrefix="expExplorer" TagName="ExpFooter" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head>
  <title>Experience Viewer</title>
  <expExplorer:ExpHeader runat="server" ID="ExpHeader" />
  <link rel="stylesheet" type="text/css" href="/sitecore modules/Web/ExperienceExplorer/Assets/css/experience-explorer-iframe-viewer.css" />
</head>
<body data-spy="scroll" data-target=".bs-docs-sidebar" class="sc ">
  <form id="form1" runat="server">
    <input type="hidden" value="<%=Sitecore.ExperienceExplorer.Business.Helpers.QueryStringHelper.GetSitecorePageSite %>" id="sitename" />
    <input type="hidden" value="<%=Sitecore.ExperienceExplorer.Business.Helpers.QueryStringHelper.GetContextItemId %>" id="currentItem" />
    <input type="hidden" value="<%=Sitecore.ExperienceExplorer.Business.Helpers.PresetsHelper.CurrentPresetId %>" id="currentPresetId" />
    <div class="experience-explorer-iframe experience-explorer-iframe-viewer">
      <div id="frame-header">
        <div class="viewer-header">
          <span class="font-xlarge light">
            <asp:Literal ID="litViewer" runat="server" /></span>
        </div>
        <table class="mode-table table border-bottom">
          <tbody>
            <tr>
              <td>
                <div class="mode">
                    <i id="JournayLbl" runat="server" class="icon-journey btn"></i>
                    <i id="FixedLbl" runat="server" class="icon-fixed btn"></i>
                  <asp:Label ID="lblModeSelected" CssClass="mode-title" runat="server"></asp:Label>
                </div>
              </td>
              <td>
                <div class="visit-counters">
                  <div class="inline-flex">
                    <table>
                      <tr>
                        <td>
                          <div class="counter-icon">
                            <i class="icon-th"></i>
                          </div>
                        </td>
                        <td>
                          <table style="margin-left: 5px;">
                            <tr>
                              <td>
                                <asp:Literal ID="ExperienceViewerViews" runat="server" />
                              </td>
                            </tr>
                            <tr>
                              <td>
                                <span class="font-small light">
                                  <asp:Literal ID="litViews" runat="server" />
                                </span>
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>
                  </div>
                  <div class="inline-flex">                   
                    <table>
                      <tr>
                        <td>
                          <div class="counter-icon">
                            <i class="icon-heart"></i>
                          </div>
                        </td>
                        <td>
                          <table style="margin-left: 5px;">
                            <tr>
                              <td>
                                <asp:Literal ID="ExperienceViewerValue" runat="server" />
                              </td>
                            </tr>
                            <tr>
                              <td>
                                <span class="font-small light">
                                  <asp:Literal ID="litValue" runat="server" />
                                </span>
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div id="accordions-viewer">
        <asp:Repeater runat="server" ID="rpAccordion" OnItemDataBound="rpAccordion_OnItemDataBound">

          <ItemTemplate>
            <div id="accordion" class="sc-advancedExpander accordion-group" runat="server">
              <div class="sc-advancedExpander-header accordion-heading accordion-heading-viewer">
                <a data-parent="#accordions-viewer" data-toggle="collapse" class="sc-advancedExpander-header-title accordion-toggle" id="accordionLink" runat="server">
                  <asp:Literal ID="litAccordionName" runat="server" />
                  <div class="sc-expander-chevron chevron"></div>
                </a>
              </div>

              <!-- tabs start -->
              <div id="accordions_body" class="sc-advancedExpander-body accordion-body" runat="server">
                <div class="accordion-inner">
                  <div class="sc-tabcontrol tabbable">
                    <asp:Repeater runat="server" ID="rpTabs" OnItemDataBound="tabs_OnItemDataBound">
                      <HeaderTemplate>
                        <ul class="sc-tabcontrol-navigation nav nav-tabs">
                      </HeaderTemplate>
                      <ItemTemplate>
                        <li id="liItem" runat="server">
                          <a data-toggle="tab" id="tabLink" runat="server">
                            <asp:Literal runat="server" ID="litTabName" />
                          </a>
                        </li>
                      </ItemTemplate>
                      <FooterTemplate></ul></FooterTemplate>
                    </asp:Repeater>

                    <!-- controls -->
                    <asp:Repeater runat="server" ID="rpControls" OnItemDataBound="rpControls_OnItemDataBound">
                      <HeaderTemplate>
                        <div class="tab-content">
                      </HeaderTemplate>
                      <ItemTemplate>
                        <div id="<%# ((Item)Container.DataItem).ID.ToShortID().ToString()%>" class="tab-pane"></div>
                      </ItemTemplate>
                      <FooterTemplate>
                        </div>
                      </FooterTemplate>
                    </asp:Repeater>
                    <!-- controls end -->

                  </div>
                </div>
              </div>
              <!-- tabs end -->

            </div>
          </ItemTemplate>

        </asp:Repeater>
      </div>

      <div class="footer-panel" id="footer-panel">
        <table class="table border-none">
          <tbody>
            <tr>
              <td>&nbsp;
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </form>

  <!-- *****************VIEWER********************* -->

  <!-- PRESET -->
  <script id="03FAF4B1CAB74306BEEF64E2E366931C_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
           
            {{if itemData.Source.Name}}
                {{for itemData.Source}}
                    <table class="table border-none table-condensed">
                      <tbody>
                        <tr>
                          <td>{{if ImageUrl}}
                                        <span class="preset-icon">
                                          <img src="{{:ImageUrl}}" alt="{{:Name}}" />
                                        </span>
                            {{/if}}
                          </td>
                          <td>
                            <h4 class="capitalize"><strong>{{:Name}}</strong></h4>
                            <p><em><small>{{:Details}}</small></em></p>
                          </td>
                        </tr>
                        {{if Description}}
                            <tr>
                              <td colspan="2"><strong>{{:~root.itemData.DescriptionLabel}}</strong></td>
                            </tr>
                        <tr>
                          <td colspan="2">{{:Description}}</td>
                        </tr>
                        {{/if}}
                      </tbody>
                    </table>

      <table class="table border-none table-condensed">
        <tbody>
          {{if Age}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerAge}}</strong></td>
                                  <td>{{:Age}}</td>
                                </tr>
          {{/if}}
                            
                            {{if DayOfMyLife}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerDayOfMyLife}}</strong></td>
                                  <td>{{:DayOfMyLife}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Education}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerEducation}}</strong></td>
                                  <td colspan="2">{{:Education}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Environment}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerEnvironment}}</strong></td>
                                  <td colspan="2">{{:Environment}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Family}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerFamily}}</strong></td>
                                  <td colspan="2">{{:Family}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Goal}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerGoal}}</strong></td>
                                  <td colspan="2">{{:Goal}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Interests}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerInterests}}</strong></td>
                                  <td colspan="2">{{:Interests}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Organization}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerOrganization}}</strong></td>
                                  <td colspan="2">{{:Organization}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Psychographics}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerPsychographics}}</strong></td>
                                  <td colspan="2">{{:Psychographics}}</td>
                                </tr>
          {{/if}}
                            
                            {{if Responsible}}
                                <tr>
                                  <td><strong>{{:~root.itemData.PresetViewerResponsible}}</strong></td>
                                  <td colspan="2">{{:Responsible}}</td>
                                </tr>
          {{/if}}

        </tbody>
      </table>
      {{/for}}

            {{else}}
                <div class="alert alert-info">{{:itemData.PresetViewerNoResults}}</div>
      {{/if}}

    </fieldset>
  </script>

  <!-- PROFILES -->
  <script id="4CAF9F9F30C7411786A41724971D5D2C_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
           
                {{for itemData.Source}} 
                    <legend class="font-large no-margin">{{:Name}}</legend>

      {{if PatternCardMatchDto != null}}
                        {{:PatternCardMatchDto.MatchPatternText}}
      <div>{{:PatternCardMatchDto.Description}}</div>
      {{else}}
                        <strong>{{:~root.itemData.NoPatternCards}}</strong>
      {{/if}}
             
                    <table class="table border-none table-condensed no-margin">
                      <tbody>
                        <tr>
                          <td>
                            <table class="table border-none padding-none no-margin">
                              <tbody>
                                {{for ProfileKeyDtos}}
                                            <tr>
                                              <td>{{:Key}}:</td>
                                              <td>{{:Value}}</td>
                                            </tr>
                                {{/for}}
                              </tbody>
                            </table>
                          </td>
                          {{if PatternCardMatchDto != null}}
                                    <td>
                                      <img class="visit-icon" src="{{:PatternCardMatchDto.ImgSrc}}" alt="{{:PatternCardMatchDto.Name}}" /></td>
                          {{/if}}
                        </tr>
                      </tbody>
                    </table>
      {{/for}}
               

    </fieldset>
  </script>

  <!-- GOALS -->
  <script id="216BFC25B59C41E999BBFF8D5F848008_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
            
            {{if itemData.Source != ""}}
                <table class="table">
                  <tbody>
                    {{for itemData.Source}}
                        <tr>
                          <td style="width:5px;">
                            <img src="{{:IconPath}}" alt="{{:Name}}" /></td>
                          <td>{{:Name}}</td>
                        </tr>
                    {{/for}}
                  </tbody>
                </table>
      {{else}}
                <div class="alert alert-info">{{:itemData.GoalNoGoalsTriggered}}</div>
      {{/if}}
    </fieldset>
  </script>

  <!-- PageEvents -->
  <script id="54ED4765A7684A1EAFE9B386D5A55E42_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
            
            {{if itemData.Source != ""}}
                <table class="table">
                  <tbody>
                    {{for itemData.Source}}
                        <tr>
                          <td style="width: 5px;">
                            <img src="{{:IconPath}}" alt="{{:Name}}" /></td>
                          <td>{{:Name}}</td>
                        </tr>
                    {{/for}}
                  </tbody>
                </table>
      {{else}}
                <div class="alert alert-info">{{:itemData.NoPageEventsTriggered}}</div>
      {{/if}}
    </fieldset>
  </script>

  <!-- DEVICE -->
  <script id="B2764CBE552F450ABCD5B1AC9A6DF3D4_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}

            {{if itemData.Source.DeviceName}}
            <p>
              {{for itemData.Source}}
                <img src="{{:ImageUrl}}" alt="{{:DeviceName}}" />
              <strong>{{:DeviceName}}</strong>
              {{/for}}
            </p>
      {{else}}
                <div class="alert alert-info">{{:itemData.NoResults}}</div>
      {{/if}}
    </fieldset>
  </script>

  <!-- GEO -->
  <script id="4420B3A22E7B411CBE40B6EA1CC4637B_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
            
            <table class="table">
              <tbody>
                {{if !itemData.isCountryTabNull }}
                    <tr>
                      <td><strong>{{:itemData.CountryCode}}</strong></td>
                      <td>{{:itemData.Source.Country}}</td>
                    </tr>
                {{/if}}
                    {{if !itemData.isIpTabNull }}
                    <tr>
                      <td><strong>{{:itemData.Ip}}</strong></td>
                      <td>{{:itemData.Source.Ip}}</td>
                    </tr>
                <tr>
                  <td><strong>{{:itemData.AreaCode}}</strong></td>
                  <td>{{:itemData.Source.AreaCode}}</td>
                </tr>
                <tr>
                  <td><strong>{{:itemData.City}}</strong></td>
                  <td>{{:itemData.Source.City}}</td>
                </tr>
                <tr>
                  <td><strong>{{:itemData.PostalCode}}</strong></td>
                  <td>{{:itemData.Source.PostalCode}}</td>
                </tr>
                <tr>
                  <td><strong>{{:itemData.BusinessName}}</strong></td>
                  <td>{{:itemData.Source.BusinessName}}</td>
                </tr>
                <tr>
                  <td><strong>{{:itemData.MetroCode}}</strong></td>
                  <td>{{:itemData.Source.MetroCode}}</td>
                </tr>
                <tr>
                  <td><strong>{{:itemData.ISP}}</strong></td>
                  <td>{{:itemData.Source.IspName}}</td>
                </tr>
                {{/if}}
                    {{if !itemData.isMapTabNull}}
 
                        {{if itemData.MapProvider}}
                            <tr>
                              <td colspan="2">
                                <div id="Map" class="map" data-name="{{:itemData.MapProvider.Name}}" data-api-key="{{:itemData.MapProvider.ApiKey}}" data-draggable="false"></div>
                              </td>
                            </tr>
                {{/if}}

                    <tr>
                      <td><strong>{{:itemData.Latitude}}</strong></td>
                      <td>{{:itemData.Source.Latitude}}
                            <input type="hidden" id="GeoLatitude" value="{{:itemData.Source.Latitude}}" />
                      </td>
                    </tr>
                <tr>
                  <td><strong>{{:itemData.Longitude}}</strong></td>
                  <td>{{:itemData.Source.Longitude}}
                            <input type="hidden" id="GeoLongitude" value="{{:itemData.Source.Longitude}}" />
                  </td>
                </tr>
                {{/if}}
              </tbody>
            </table>

    </fieldset>
  </script>

  <!-- TAGS -->
  <script id="194BE1EA801445F4BE05B2D549DD4BAD_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
            
            {{if itemData.Source != ""}}
                <table class="table">
                  <tbody>
                    {{for itemData.Source}}
                        <tr>
                          <td>
                            <img src="/sitecore modules/web/experienceexplorer/assets/images/check-icon.gif" alt="Tag" /></td>
                          <td><strong>{{:TagName}}:</strong></td>
                          <td class="nestedList">{{for TagValues}}
                                <div>{{>#data}}</div>
                            {{/for}}
                          </td>
                        </tr>
                    {{/for}}
                  </tbody>
                </table>
      {{else}}
                <div class="alert alert-info">{{:itemData.NoResults}}</div>
      {{/if}}
    </fieldset>
  </script>

  <!-- CAMPAIGNS -->
  <script id="89C93B33BAFF4F03950ED3031A85CB9B_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
            
            {{if itemData.Source != ""}}
                {{for itemData.Source}}
                    <label>
                      <img src="/sitecore modules/web/experienceexplorer/assets/images/check-icon.gif" alt="goal" />
                      <span>{{:Name}}</span>
                    </label>
      {{/for}}
            {{else}}
                <div class="alert alert-info">{{:~root.itemData.NoResults}}</div>
      {{/if}}
    </fieldset>
  </script>

  <!-- REFERRALS -->
  <script id="B5FA229A67D343BBAFC61A1118C9CEC0_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
            
            {{if itemData.Source.Referrer != ""}}
                <p class="referral-link">
                  <strong>{{:itemData.ReferralViewerReferrer}}:</strong><br />
                  {{:itemData.Source.Referrer}}
                </p>

      {{if itemData.Source.ReferringSiteDto.Host != null}}
                    <p>
                      <strong>{{:itemData.ReferralViewerSiteHost}}</strong><br />
                      {{:itemData.Source.ReferringSiteDto.Host}}
                    </p>
      {{/if}}
            <p>
              <strong>{{:itemData.ReferralViewerKeywords}}</strong><br />
              <ul>
                {{for itemData.Source.Keywords}}
                     <li>{{:Keyword}}</li>
                {{/for}}
              </ul>
            </p>
      {{else}}
                <div class="alert alert-info">{{:itemData.NoResults}}</div>
      {{/if}}
    </fieldset>
  </script>

  <!-- RULES -->
  <script id="A5A2D84AF35E4283A07E201E5E0DE4E2_view" type="text/x-jsrender">
    <fieldset class="tab-fieldset">
      {{include tmpl="#HeaderTmpl"/}}
            
            <ul class="rules unstyled">
              {{for itemData.Source}}
                <li>
                  <img src="{{:Icon}}" alt="{{:Name}}" />
                  <span>{{:Name}}</span>

                  {{for RuleDtos}}
                        <label data-toggle="tooltip" class="conditions">
                          {{if Type == 'green' && Selected}}  
                                <img alt="" src="/sitecore modules/web/experienceExplorer/assets/images/bullet_circle_green.png" />
                          {{else}}

                                {{if Type == 'yellow' && Selected}}
                                    <img alt="" src="/sitecore modules/web/experienceExplorer/assets/images/bullet_circle_yellow.png" />
                                {{else}}
                                    <img alt="" src="/sitecore modules/web/experienceExplorer/assets/images/bullet_circle_red.png" />
                                {{/if}}

                          {{/if}}
                         <span>{{:Name}}</span>
                        </label>
                  <div class="conditions-title hidden">
                        {{for Conditions}}
                          {{if Indent}}
                            <p class="nested">
                            {{if IsTrue}}  
                              <span class="green-indication"></span>
                            {{else}}
                              <span class="red-indication"></span>
                            {{/if}}
                            <span>{{:Text}}</span>
                            </p>
                          {{else}}
                            <p>
                            {{if IsTrue}}  
                              <span class="green-indication"></span>
                            {{else}}
                              <span class="red-indication"></span>
                            {{/if}}
                            <span>{{:Text}}</span>
                            </p>
                          {{/if}}
                        {{/for}} <!-- End "Conditions" for -->
                        </div>
                        
                      {{/for}} <!-- End "RuleDtos" for -->
                    </li>
                  {{/for}} <!-- End "itemData.Source" for -->
               </ul>
    </fieldset>
  </script>

  <expExplorer:ExpFooter runat="server" ID="expFooter" />
</body>
</html>
