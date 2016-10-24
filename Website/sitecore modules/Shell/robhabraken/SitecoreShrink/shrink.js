

// Renders a donut chart using D3 on the div with ID chartId, using the number jsonObjectIndex item from the given media library report json file.
function renderDonut(chartId, jsonFilePath, jsonObjectIndex) {

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
function arcTweenZoom(d) {
  var xd = d3.interpolate(x.domain(), [d.x, d.x + d.dx]),
      yd = d3.interpolate(y.domain(), [d.y, 1]),
      yr = d3.interpolate(y.range(), [d.y ? 20 : 0, radius]);
  return function(d, i) {
    return i
        ? function(t) { return arc(d); }
        : function(t) { x.domain(xd(t)); y.domain(yd(t)).range(yr(t)); return arc(d); };
  };
}