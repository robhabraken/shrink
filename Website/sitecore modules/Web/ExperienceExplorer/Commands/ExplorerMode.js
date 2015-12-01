define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
  Sitecore.Commands.ExplorerMode =
  {
    canExecute: function (context) {
      return context.app.canExecute("ExperienceExplorer.ExplorerMode.CanExplore", context.currentContext);
    },

    execute: function (context) {
      context.currentContext.value = encodeURIComponent(window.parent.location);
      ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceExplorer.ExplorerMode.Explore", function (response) {
        window.parent.location = response.responseValue.value;
      }).execute(context);
    }
  };
});