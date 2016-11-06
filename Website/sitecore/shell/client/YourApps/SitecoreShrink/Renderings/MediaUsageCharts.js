define(['sitecore'], function (_sc) {
    var control = {
        componentName: "MediaUsageCharts",
        selector: ".sc-mediausagecharts",
        control: "dynatree",
        namespace: "ui-",

        attributes:
        [
          { name: "appContext", defaultValue: "" }
        ],

        events:
        [
        ],

        functions:
        [
        ],

        view:
        {
            initialized: function () {
                this.model.set("appContext", this.$el.attr("data-sc-appcontext"));

                var mediaItemPath = this.invalidateCache("/sitecore/shell/client/YourApps/SitecoreShrink/DataStorage/mediaitems.json");
                var libraryReportPath = this.invalidateCache("/sitecore/shell/client/YourApps/SitecoreShrink/DataStorage/libraryreport.json");

                this.renderSunburst("media-usage", mediaItemPath, this.model);
                this.renderDonut("referenced-chart", libraryReportPath, 0, "size-referenced", 1, true, this.model);
                this.renderDonut("published-chart", libraryReportPath, 1, "size-published", 1, true, this.model);
                this.renderDonut("versions-chart", libraryReportPath, 2, "size-versions", 0, false, this.model);
            },

            invalidateCache: function (url) {
                return url + "?" + new Date().getTime();
            },

            // Renders a sunburst chart using D3 on the div with ID chartId, using the given media item report json file.
            renderSunburst: function (chartId, jsonFilePath, appContext) {

                var width = 780,
                    height = 580,
                    radius = Math.min(width, height) / 2;

                var x = d3.scale.linear()
                    .range([0, 2 * Math.PI]);

                var y = d3.scale.sqrt()
                    .range([0, radius]);

                var color = d3.scale.category20c();

                var svg = d3.select("#" + chartId).append("svg")
                    .attr("preserveAspectRatio", "xMinYMin meet")
                    .attr("viewBox", "0 0 " + width + " " + height)
                    .attr("class", "svg")
                  .append("g")
                    .attr("transform", "translate(" + width / 2 + "," + (height / 2) + ")");

                var partition = d3.layout.partition()
                    .sort(null)
                    .value(function (d) { return 1; });

                var arc = d3.svg.arc()
                    .startAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x))); })
                    .endAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x + d.dx))); })
                    .innerRadius(function (d) { return Math.max(0, y(d.y)); })
                    .outerRadius(function (d) { return Math.max(0, y(d.y + d.dy)); });

                // Keep track of the node that is currently being displayed as the root.
                var node;

                d3.json(jsonFilePath, function (error, root) {
                    node = root;
                    var path = svg.datum(root).selectAll("path")
                        .data(partition.value(function (d) { return d.size; }).nodes)
                      .enter().append("path")
                        .attr("d", arc)
                        .style("fill", function (d) { return color((d.children ? d : d.parent).name); })
                        .on("click", click)
                      .append("title")
                        .text(function (d) { return d.name + "\n" + toReadableFileSizeString(d.value); });

                    function click(d) {
                        node = d;
                        svg.transition()
                          .duration(1000)
                        .selectAll("path")
                          .attrTween("d", arcTweenZoom(d, x, y, radius, arc));

                        var app = appContext.get("appContext");

                        jQuery.ajax({
                            type: "GET",
                            dataType: "json",
                            url: "/api/sitecore/MediaDashboard/ZoomIn",
                            data: { 'id': d.id },
                            cache: false,
                            success: function (data) {
                                app.clickSunburst(app, data);
                            },
                            error: function () {
                                console.log("There was an error. Try again please!");
                            }
                        });
                    }
                });
            },

            // Renders a donut chart using D3 on the div with ID chartId, using the number jsonObjectIndex item from the given media library report json file.
            // This script also puts the formatted statistics with the highlight stat index into the div with ID hightlighId (most important metric).
            // If sizeInBytes is true, the sizes of the donut chart are stored in bytes in the given json file, otherwise whole numbers are used (item count).
            renderDonut: function (chartId, jsonFilePath, jsonObjectIndex, highlightId, highlightStatIndex, sizeInBytes, appContext) {

                var width = 250,
                    height = 250,
                    radius = Math.min(width, height) / 2;

                var color = d3.scale.category20c();

                var arc = d3.svg.arc()
                    .outerRadius(radius - 10)
                    .innerRadius(radius - 50);

                var pie = d3.layout.pie()
                    .sort(null)
                    .value(function (d) { return d.size; });

                var svg = d3.select("#" + chartId).append("svg")
                    .attr("preserveAspectRatio", "xMinYMin meet")
                    .attr("viewBox", "0 0 " + width + " " + height)
                    .attr("class", "svg")
                  .append("g")
                    .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

                d3.json(jsonFilePath, function (error, data) {
                    if (error) throw error;

                    var g = svg.selectAll(".arc")
                        .data(pie(data.stats[jsonObjectIndex].children))
                      .enter().append("g")
                        .attr("class", "arc");

                    g.append("path")
                        .attr("d", arc)
                        .style("fill", function (d) { return color(d.data.category); })
                        .on("click", click)
                      .append("title")
                        .text(function (d) { return d.data.category + "\n" + toReadableFileSizeString(d.data.size, !sizeInBytes); });

                    var highlightStatistic = data.stats[jsonObjectIndex].children[highlightStatIndex].size;
                    d3.select("#" + highlightId).text(toReadableFileSizeString(highlightStatistic, !sizeInBytes));

                    function click(d) {
                        var category = d.data.category;
                        var app = appContext.get("appContext");

                        jQuery.ajax({
                            type: "GET",
                            dataType: "json",
                            url: "/api/sitecore/MediaDashboard/SelectSubset",
                            data: { 'category': category },
                            cache: false,
                            success: function (data) {
                                app.clickDonut(app, category, data);
                            },
                            error: function () {
                                console.log("There was an error. Try again please!");
                            }
                        });
                    }
                });
            }
        }
    };

    _sc.Factories.createJQueryUIComponent(_sc.Definitions.Models, _sc.Definitions.Views, control);

    // Format the size of an item or a category from bytes to a formatted and human readable file size string.
    // The skip boolean indicates that the given size concerns whole numbers instead of a file size in bytes, so this method should be skipped.
    // Editorial note: I know this exception is kind of cheesy, but this was far less complex to read than making exceptions in the calling method.
    function toReadableFileSizeString(fileSizeInBytes, skip) {
        if (skip) {
            return fileSizeInBytes;
        }

        var i = -1;
        var byteUnits = [" kB", " MB", " GB", " TB", " PB", " EB", " ZB", " YB"];
        do {
            fileSizeInBytes = fileSizeInBytes / 1024;
            i++;
        } while (fileSizeInBytes > 1024);

        return Math.max(fileSizeInBytes, 0.1).toFixed(1) + byteUnits[i];
    }

    // Sunburst chart helper function: when zooming, interpolate the scales.
    function arcTweenZoom(d, x, y, radius, arc) {
        var xd = d3.interpolate(x.domain(), [d.x, d.x + d.dx]),
            yd = d3.interpolate(y.domain(), [d.y, 1]),
            yr = d3.interpolate(y.range(), [d.y ? 20 : 0, radius]);
        return function (d, i) {
            return i
                ? function (t) { return arc(d); }
                : function (t) { x.domain(xd(t)); y.domain(yd(t)).range(yr(t)); return arc(d); };
        };
    }
});


