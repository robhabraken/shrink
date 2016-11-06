define(["sitecore"], function (Sitecore) {
    var ShrinkMediaDashboard = Sitecore.Definitions.App.extend({
        initialized: function () {
            var app = this;

            // pass a reference of app context to the chart component to be able to handle
            // onclicks events within the charts triggering updates of other components
            app.MediaCharts.set("appContext", app);

            // flow step 1: if there are active jobs running, the job progress window should be displayed
            app.checkActiveJobs();
        },
        checkActiveJobs: function () {
            var app = this;

            jQuery.ajax({
                type: "GET",
                dataType: "json",
                url: "/api/sitecore/MediaDashboard/GetActiveJobDescription",
                data: { },
                cache: false,
                success: function (data) {
                    var jobDescription = data[0];
                    var itemsProcessed = data[1];
                    var totalItemCount = data[2];

                    if (jobDescription) {
                        app.showJobDialog(jobDescription, itemsProcessed, totalItemCount);
                    } else {
                        app.hideJobDialog();

                        // flow step 2: if there are no active jobs, check if the json data is available
                        // if not, scan media (also bound to corresponding button in interface) starts immediately
                        if (!app.dataStorageAvailable()) {
                            app.scanMedia();
                        }

                        // flow step 3: if there is no job running and the data is present, the dashboard is shown
                    }
                },
                error: function () {
                    console.log("There was an error. Try again please!");
                }
            });
        },
        dataStorageAvailable: function () {
            var app = this;
            return app.fileExists(app.invalidateCache("/sitecore/shell/client/YourApps/SitecoreShrink/DataStorage/mediaitems.json"))
                && app.fileExists(app.invalidateCache("/sitecore/shell/client/YourApps/SitecoreShrink/DataStorage/libraryreport.json"));
        },
        fileExists: function (url) {
            var http = new XMLHttpRequest();
            http.open('HEAD', url, false);
            http.send();
            return http.status != 404;
        },
        invalidateCache: function (url) {
            return url + "?" + new Date().getTime();
        },
        showJobDialog: function(friendlyJobName, itemsProcessed, totalItemCount) {
            var app = this;
            app.Main.set("isVisible", false);
            app.ScanButton.set("isVisible", false);
            app.JobDescription.set("text", friendlyJobName + "...");
            app.JobBorder.set("isVisible", true);

            if (totalItemCount > 0) {
                app.JobProgressBar.set("isVisible", true);
                app.JobProgressBar.set("value", itemsProcessed);
                app.JobProgressBar.set("maxValue", totalItemCount);
            } else {
                app.JobProgressBar.set("isVisible", false);
            }

            // re-check active jobs to hide the dialog when finished
            setTimeout(app.checkActiveJobs(), 5000);
        },
        hideJobDialog: function() {
            var app = this;

            // toggling back after a job is finished requires the screen to refresh once to load all the updated data
            if (app.JobBorder.get("isVisible") == true) {
                location.reload();
            } else {
                app.JobBorder.set("isVisible", false);
                app.Main.set("isVisible", true);
                app.ScanButton.set("isVisible", true);
            }
        },
        clickSunburst: function (app, data) {
            // before we are going to load the new key path, we need to reload the tree component
            app.SelectedMediaItemTreeView.viewModel.reload();

            // get the item path in Sitecore IDs to the selected item in the sunburst chart
            // and load that path in the treeview component to match up the views
            app.SelectedMediaItemTreeView.viewModel.pathToLoad(data);
            app.SelectedMediaItemTreeView.viewModel.loadKeyPath();
        },
        clickDonut: function (app, category, data) {
            // pass a list of item IDs of all items that belong to this category to the treeview component
            // so the treeview component is able to filter its view based on this ID list
            app.SelectedMediaItemTreeView.set("itemsToDisplay", data);
            app.SelectedMediaItemTreeView.viewModel.reload();

            // add a message bar to notify the user of the active subset of the data
            app.SubsetMessageBorder.set("isVisible", true);
            app.SubsetMessageBar.removeMessages();
            app.SubsetMessageBar.addMessage("notification", {
                text: "You are now viewing all " + category.toLowerCase(),
                actions: [{ text: "Refresh", action: "javascript: window.location.reload(false);" }],
                closable: false
            });
        },
        expandAllNodes: function() {
            var app = this;
            app.SelectedMediaItemTreeView.viewModel.expandAll();
        },
        scanMedia: function () {
            var app = this;

            jQuery.ajax({
                type: "GET",
                dataType: "json",
                url: "/api/sitecore/MediaDashboard/ScanMedia",
                data: { },
                cache: false,
                success: function (data) {
                    app.checkActiveJobs();
                },
                error: function () {
                    console.log("There was an error. Try again please!");
                }
            });
        },
        archiveMedia: function () {
            var app = this;

            var items = app.SelectedMediaItemTreeView.get("checkedItemIds");

            jQuery.ajax({
                type: "POST",
                dataType: "json",
                url: "/api/sitecore/MediaDashboard/ArchiveMedia",
                data: { 'itemList': items },
                cache: false,
                success: function (data) {
                    app.checkActiveJobs();
                },
                error: function () {
                    console.log("There was an error. Try again please!");
                }
            });
        },
        recycleMedia: function () {
            var app = this;

            var items = app.SelectedMediaItemTreeView.get("checkedItemIds");

            jQuery.ajax({
                type: "POST",
                dataType: "json",
                url: "/api/sitecore/MediaDashboard/RecycleMedia",
                data: { 'itemList': items },
                cache: false,
                success: function (data) {
                    app.checkActiveJobs();
                },
                error: function () {
                    console.log("There was an error. Try again please!");
                }
            });
        },
        deleteMedia: function () {
            var app = this;

            var items = app.SelectedMediaItemTreeView.get("checkedItemIds");

            jQuery.ajax({
                type: "POST",
                dataType: "json",
                url: "/api/sitecore/MediaDashboard/DeleteMedia",
                data: { 'itemList': items },
                cache: false,
                success: function (data) {
                    app.checkActiveJobs();
                },
                error: function () {
                    console.log("There was an error. Try again please!");
                }
            });
        },
        downloadMedia: function () {
            var app = this;
            
            var items = app.SelectedMediaItemTreeView.get("checkedItemIds");
            var deleteAfterwards = app.DeleteCheckbox.get("isChecked");

            jQuery.ajax({
                type: "POST",
                dataType: "json",
                url: "/api/sitecore/MediaDashboard/DownloadMedia",
                data: { 'itemList': items, 'deleteAfterwards': deleteAfterwards },
                cache: false,
                success: function (data) {
                    app.checkActiveJobs();
                },
                error: function () {
                    console.log("There was an error. Try again please!");
                }
            });
        },
        deleteOldVersions: function () {
            var app = this;

            var items = app.SelectedMediaItemTreeView.get("checkedItemIds");

            jQuery.ajax({
                type: "POST",
                dataType: "json",
                url: "/api/sitecore/MediaDashboard/DeleteOldVersions",
                data: { 'itemList': items },
                cache: false,
                success: function (data) {
                    app.checkActiveJobs();
                },
                error: function () {
                    console.log("There was an error. Try again please!");
                }
            });
        }
    });
    return ShrinkMediaDashboard;
});