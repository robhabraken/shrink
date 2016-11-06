/// <reference path="../../../../../../assets/lib/dist/sitecore.js" />
/// <reference path="../../../../../../assets/vendors/underscore/underscore.js" />

require.config({
  paths: {
      dynatree: "/sitecore/shell/client/Speak/Assets/lib/ui/1.1/deps/DynaTree/jquery.dynatree-1.2.4",
      dynatreecss: "/sitecore/shell/client/Speak/Assets/lib/ui/1.1/deps/DynaTree/skin-vista/ui.dynatree"
  },    
  shim: {
      "dynatree": { deps: [ "jqueryui" ] }
  }
});

define(['sitecore', 'dynatree'], function (_sc) {
  var control = {
    componentName: "ItemTreeView",
    selector: ".sc-itemtreeview",
    control: "dynatree",
    namespace: "ui-",

    attributes:
    [
      { name: "selectedItemId", added: true },
      { name: "selectedItemPath", added: true },
      { name: "checkedItemIds", added: true },
      { name: "pathToLoad", added: true },
      { name: "isBusy", added: true },
      { name: "selectedNode", added: true },
      { name: "showSitecoreIcons", added: true, defaultValue: true },
      { name: "isCheckboxEnabled", pluginProperty: "checkbox", defaultValue: true },     // Show checkboxes.
      { name: "isKeyboardSupported", pluginProperty: "keyboard", defaultValue: true },     // Support keyboard navigation.
      { name: "isPersist", pluginProperty: "persist", defaultValue: false },      // Persist expand-status to a cookie
      { name: "isAutoFocus", pluginProperty: "autoFocus", defaultValue: true },    // Set focus to first child, when expanding or lazy-loading.
      { name: "isAutoCollapse", pluginProperty: "autoCollapse", defaultValue: false }, // Automatically collapse all siblings, when a node is expanded.
      { name: "clickFolderMode", defaultValue: 1 }, // 1:activate, 2:expand, 3:activate and expand
      { name: "selectMode", defaultValue: 3 },      // 1:single, 2:multi, 3:multi-hier
      { name: "isNoLink", pluginProperty: "noLink", defaultValue: false },       // Use <span> instead of <a> tags for all nodes
      { name: "debugLevel", defaultValue: 0 },       // 0:quiet, 1:normal, 2:debug
      { name: "showHiddenItems", defaultValue: false },
      { name: "contentLanguage", defaultValue: "en" },
      { name: "showIconImage", defaultValue: true },
      { name: "database", defaultValue: "" },

      // SelectableTreeView addition
      { name: "itemsToDisplay", defaultValue: ""}
    ],

    events:
    [
      { name: "onPostInit" },        // Callback(isReloading, isError) when tree was (re)loaded.
      { name: "onActivate", on: "onActivate" },        // Callback(dtnode) when a node is activated.
      { name: "onDeactivate" },      // Callback(dtnode) when a node is deactivated.
      { name: "onSelect", on: "onSelect" },          // Callback(flag, dtnode) when a node is (de)selected.
      { name: "onExpand" },          // Callback(flag, dtnode) when a node is expanded/collapsed.
      { name: "onLazyRead", on: "nodeExpanding" },   // Callback(dtnode) when a lazy node is expanded for the first time.
      { name: "onCustomRender" },    // Callback(dtnode) before a node is rendered. Return a HTML string to override.
      { name: "onCreate" },          // Callback(dtnode, nodeSpan) after a node was rendered for the first time.
      { name: "onRender" },          // Callback(dtnode, nodeSpan) after a node was rendered.
      { name: "postProcess" }        // Callback(data, dataType) before an Ajax result is passed to dynatree.
    ],

    functions:
    [
      { name: "disable" },
      { name: "enable" },
      { name: "getTree" },
      { name: "getRoot" },
      { name: "getActiveNode" },
      { name: "getSelectedNodes" }
    ],

    view:
    {
      initialized: function () {
        var clickFolderModeAttribute = this.$el.attr("data-sc-option-clickfoldermode"),
          selectModeAttribute = this.$el.attr("data-sc-option-selectmode");

        this.model.set("clickFolderMode", clickFolderModeAttribute && !isNaN(clickFolderModeAttribute) ? parseInt(clickFolderModeAttribute, 10) : 1);
        this.model.set("selectMode", selectModeAttribute && !isNaN(selectModeAttribute) ? parseInt(selectModeAttribute, 10) : 3);
        this.model.set("contentLanguage", this.$el.attr("data-sc-contentlanguage"));
        this.model.set("showHiddenItems", this.$el.attr("data-sc-option-showhiddenitems") !== undefined && this.$el.attr("data-sc-option-showhiddenitems").toLowerCase() === "true");
        this.model.set("showIconImage", this.$el.attr("data-sc-option-showiconimage") === undefined || this.$el.attr("data-sc-option-showiconimage") === "true");
        this.model.set("templates", this.$el.attr("data-sc-templates"));
        this.model.set("database", this.$el.attr("data-sc-database"));

        var items = this.$el.attr("data-sc-rootitem");
        if (!items) {
          return;
        }

        var pathToLoad = this.$el.attr("data-sc-loadpath");
        if (pathToLoad) {
          this.model.set("pathToLoad", pathToLoad);
        }

        var parts = items.split("|");
        var selectedItemUri = null;

        for (var n = 0; n < parts.length; n++) {
          var uri = parts[n].split(',');

          var databaseUri = new _sc.Definitions.Data.DatabaseUri(uri[1]);
          var itemUri = new _sc.Definitions.Data.ItemUri(databaseUri, uri[2]);
          var itemIcon = uri[3];

          var rootNode = {
            title: uri[0],
            key: itemUri.getItemId(),
            isLazy: true,
            isFolder: true,
            icon: this.model.get("showSitecoreIcons") ? itemIcon : "",
            url: "#",
            path: this.$el.attr("data-sc-rootitempath"),
            itemUri: itemUri,
            selected: selectedItemUri == null
          };

          if (this.widget) {
            this.widget.apply(this.$el, ["getRoot"]).addChild(rootNode);
          }

          if (this.model.get("pathToLoad") && this.model.get("pathToLoad") !== "") {
            this.loadKeyPath();
          }
        }

        this.pendingRequests = 0;
      },

      onActivate: function (node) {
        if (node && node.data && node.data.itemUri && node.data.path) {
          this.model.set("selectedNode", node.data);
          this.model.set("selectedItemId", node.data.itemUri.itemId);
          this.model.set("selectedItemPath", node.data.path);
        }
      },

      onSelect: function (flag, node) {
        var list = [];

        _.each(node.tree.getSelectedNodes(), function (dnode, index) {
          list.push(dnode.data.itemUri.itemId);
        });

        this.model.set("checkedItemIds", list.join("|"));
      },

      nodeExpanding: function (node) {
        var itemUri = node.data.itemUri;

        this.pendingRequests++;
        this.model.set("isBusy", true);

        var self = this;

        // SelectableTreeView addition
        var itemsToDisplay = new Array();
        if (this.model.get("itemsToDisplay") !== undefined && this.model.get("itemsToDisplay") != "") {
          itemsToDisplay = this.model.get("itemsToDisplay").split("|");
        }

        var database = new _sc.Definitions.Data.Database(itemUri.getDatabaseUri());
        database.getChildren(itemUri.getItemId(), function (items, totalCount, result) {

          if (result.statusCode === 401) {
            _sc.Helpers.session.unauthorized();
            return;
          }
          
          var res = [], filteredItems;

          filteredItems = _.filter(items, function (item) {
            // SelectableTreeView modification: do not show items that are not included in the itemsToDisplay list, except when an item is a media folder,
            // but only when the itemsToDisplay field is filled - otherwise, return to default behavior of TreeView SPEAK component 
            if (itemsToDisplay.length > 0) {
              return ($.inArray(item.itemId.toLowerCase(), itemsToDisplay) >= 0 || item.$templateId === "{FE5DD826-48C6-436D-B87A-7C4210C7413B}");
            } else {
              return true;
            }
          });

          if (self.model.get("templates")) {
            var templateList = self.model.get("templates");
            filteredItems = _.filter(filteredItems, function (item) {
              return templateList.indexOf(item.$templateId, 0) != -1;
            });
          }

          self.appendLoadedChildren(node, filteredItems, res);

          self.pendingRequests--;
          if (self.pendingRequests <= 0) {
            self.pendingRequests = 0;
            self.model.set("isBusy", false);
          }

          if (self.model.get("pathToLoad") && self.model.get("pathToLoad").length > 0) {
            self.loadKeyPath();
          }
        }, {
          fields: ["__Hidden"],
          language: self.model.get("contentLanguage")
        });
      },

      appendLanguageParameter: function (item) {
        if (item.$icon.indexOf(".ashx") > 0) {
          item.$icon += "&la=" + this.model.get("contentLanguage");
          item.$mediaurl += "&la=" + this.model.get("contentLanguage");
        }
      },

      appendLoadedChildren: function (parentNode, childrenNodes, destArray) {
        //add children

        var self = this;
        _.each(childrenNodes, function (item) {
          var newNode = {};
          newNode.rawItem = item;
          newNode.title = item.$displayName;
          newNode.key = item.itemId;
          if (self.model.get("showSitecoreIcons")) {
            self.appendLanguageParameter(item);
            newNode.icon = item.$icon;
          }
          newNode.itemUri = item.itemUri;
          newNode.path = item.$path;

          // don't do this, because it's not true or correct: copying the select value implies the child item was selected before expanding,
          // but it wasn't, so now it appears to be selected, but it isn't included in the selectedItem list, so let's omit this line
          // newNode.select = self.model.get("selectMode") === 3 ? parentNode.isSelected() : false;

          newNode.isFolder = item.$hasChildren;
          newNode.isLazy = item.$hasChildren;
          destArray.push(newNode);

        }, this);

        parentNode.setLazyNodeStatus(DTNodeStatus_Ok);
        parentNode.addChild(destArray);
        //expand needed node
      },

      // this function reloads the tree completely by removing and reinitializing it, to reset any previous selections
      // the default tree.reload function created a double root item each time it was called, so I couldn't use that
      reload: function () {
        var tree = this.widget.apply(this.$el, ["getTree"]);
        tree.getRoot().removeChildren();
        this.initialized();
      },

      expandAll: function () {
        var tree = this.widget.apply(this.$el, ["getTree"]);
        tree.getRoot().visit(function(node) {
          node.expand(true);
        });
      },

      loadKeyPath: function () {
        var separator = "/",
            pathParts,
            currentNodeId,
            path = this.model.get("pathToLoad"),
            tree = this.widget.apply(this.$el, ["getTree"]),
            node;

        pathParts = path.split(separator);
        if (pathParts.length === 0) {
          return false;
        }

        currentNodeId = pathParts.shift();
        if (!currentNodeId) {
          return false;
        }

        node = tree.getNodeByKey(currentNodeId);
        if (!node) {
          this.model.set("pathToLoad", "");
          return false;
        }

        this.model.set("pathToLoad", pathParts.join(separator));
        node.expand();

        if (pathParts.length === 0) {
          this.model.set("selectedItemId", currentNodeId);
          node.activate(true);

          // don't do this, because it's not true or correct: selecting the value implies it is included in the selectedItem list,
          // but it isn't, so let's omit this line...
          // node.select(true);
        }
      }
    }
  };

  _sc.Factories.createJQueryUIComponent(_sc.Definitions.Models, _sc.Definitions.Views, control);
});
