
// Renders a sunburst chart using D3 on the div with ID chartId, using the given media item report json file.
function renderSunburst(chartId, jsonFilePath) {

  var width = 780,
      height = 580,
      radius = Math.min(width, height) / 2;

  var x = d3.scale.linear()
      .range([0, 2 * Math.PI]);

  var y = d3.scale.sqrt()
      .range([0, radius]);

  var color = d3.scale.category20c();

  var svg = d3.select("#" + chartId).append("svg")
      .attr("width", width)
      .attr("height", height)
    .append("g")
      .attr("transform", "translate(" + width / 2 + "," + (height / 2 + 10) + ")");

  var partition = d3.layout.partition()
      .sort(null)
      .value(function(d) { return 1; });

  var arc = d3.svg.arc()
      .startAngle(function(d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x))); })
      .endAngle(function(d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x + d.dx))); })
      .innerRadius(function(d) { return Math.max(0, y(d.y)); })
      .outerRadius(function(d) { return Math.max(0, y(d.y + d.dy)); });

  // Keep track of the node that is currently being displayed as the root.
  var node;

  d3.json(jsonFilePath, function(error, root) {
    node = root;
    var path = svg.datum(root).selectAll("path")
        .data(partition.value(function(d) { return d.size; }).nodes)
      .enter().append("path")
        .attr("d", arc)
        .style("fill", function(d) { return color((d.children ? d : d.parent).name); })
        .on("click", click)
      .append("title")
        .text(function(d) { return d.name + "\n" + toReadableFileSizeString(d.value); });

    function click(d) {
      node = d;
      svg.transition()
        .duration(1000)
      .selectAll("path")
        .attrTween("d", arcTweenZoom(d, x, y, radius, arc));
    }
  });
}

// Renders a donut chart using D3 on the div with ID chartId, using the number jsonObjectIndex item from the given media library report json file.
// This script also puts the formatted statistics with the highlight stat index into the div with ID hightlighId.
function renderDonut(chartId, jsonFilePath, jsonObjectIndex, highlightId, highlightStatIndex) {

  var width = 250,
      height = 250,
      radius = Math.min(width, height) / 2;

  var color = d3.scale.category20c();

  var arc = d3.svg.arc()
      .outerRadius(radius - 10)
      .innerRadius(radius - 50);

  var pie = d3.layout.pie()
      .sort(null)
      .value(function(d) { return d.size; });

  var svg = d3.select("#" + chartId).append("svg")
      .attr("width", width)
      .attr("height", height)
    .append("g")
      .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

  d3.json(jsonFilePath, function(error, data) {
    if (error) throw error;
    
    var g = svg.selectAll(".arc")
        .data(pie(data.stats[jsonObjectIndex].children))
      .enter().append("g")
        .attr("class", "arc");

    g.append("path")
        .attr("d", arc)
        .style("fill", function(d) { return color(d.data.category); })
        .on("click", click)
      .append("title")
        .text(function(d) { return d.data.category + "\n" + toReadableFileSizeString(d.data.size); });

    var highlightStatistic = data.stats[jsonObjectIndex].children[highlightStatIndex].size;
    d3.select("#" + highlightId).text(toReadableFileSizeString(highlightStatistic));

    function click(d) {
      node = d;
      alert(d.data.category);
    }
  });
}

// Format the size of an item or a category from bytes to a formatted and human readable file size string.
function toReadableFileSizeString(fileSizeInBytes) {

    var i = -1;
    var byteUnits = [' kB', ' MB', ' GB', ' TB', ' PB', ' EB', ' ZB', ' YB'];
    do {
        fileSizeInBytes = fileSizeInBytes / 1024;
        i++;
    } while (fileSizeInBytes > 1024);

    return Math.max(fileSizeInBytes, 0.1).toFixed(1) + byteUnits[i];
};

// Sunburst chart helper function: when zooming, interpolate the scales.
function arcTweenZoom(d, x, y, radius, arc) {
  var xd = d3.interpolate(x.domain(), [d.x, d.x + d.dx]),
      yd = d3.interpolate(y.domain(), [d.y, 1]),
      yr = d3.interpolate(y.range(), [d.y ? 20 : 0, radius]);
  return function(d, i) {
    return i
        ? function(t) { return arc(d); }
        : function(t) { x.domain(xd(t)); y.domain(yd(t)).range(yr(t)); return arc(d); };
  };
}