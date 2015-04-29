
var app = angular.module("app", []);
app.controller("wages", function ($scope, $http) {


    var width = 550,
             height = 350,
             centered;

    //year data for select control
    $scope.years = [];
    $scope.currentData = [];
    for (var i = 2001; i < 2014; i++) {
        $scope.years.push(i);
    }
    $scope.selectedYear = $scope.years[12]

    $scope.areaName = "Statewide";
    //send initial data
    $http.post('Wages/data', { areaName: $scope.areaName, year: $scope.selectedYear })
            .success(function (response) {
                $scope.data = response;
                initialPie(response);
                initialColumn(response);

            });

    //Create D3 initialization variables for zoomable map
    var projection = d3.geo.albers()
            .rotate([97, .55, 5])
            .center([-12, 38.7])
            .parallels([37, 41])
            .scale(4570)
            .translate([width / 2, height / 2])
            .precision(.1);

    var path = d3.geo.path()
        .projection(projection);

    //add an SVG element to DOM

    var svg = d3.select("#mapContainer").append("svg")
        .attr("width", width)
        .attr("height", height);

    svg.append("rect")
            .attr("class", "background")
            .attr("width", width)
            .attr("height", height)
            .on("click", clicked);

    var g = svg.append("g");


    //retrieve Counties.geojson file and build paths for map

    d3.json("Content/Counties.geojson", function (error, json) {
        g.append("g")
                .attr("id", "counties")
                .selectAll("path")
                .data(json.features)
                .enter()
                .append("path")
                .attr("d", path)
                .style("stroke", "black")
                .on("click", clicked);
        g.selectAll("text")
                .data(json.features)
                .enter()
                .append("svg:text")
                .text(function (d) { return d.properties.NAME10; })
                .attr("x", function (d) { return path.centroid(d)[0]; })
                .attr("y", function (d) { return path.centroid(d)[1]; })
                .attr("text-anchor", "middle")
                .attr("font-size", "8px");

    });


    //create d3 initialization variables for Pie Chart


    var pieData = [];

    radius = Math.min(width, height) / 2;

    var arc = d3.svg.arc()
        .outerRadius(radius - 10)
        .innerRadius(0);

    var outerArc = d3.svg.arc()
	    .innerRadius(radius * 0.9)
	    .outerRadius(radius * 0.9);

    var pie = d3.layout.pie()
        .sort(null)
        .value(function (d) { return d.value; });

    var pieSvg = d3.select(".topRight")
        .append("svg")
        .attr("width", width - 50)
        .attr("height", height)
        .append("g")
        .attr("transform", "translate(" + width / 2.2 + "," + height / 2 + ")");


    //create d3 initialization variables for column chart


    var margin = { top: 4, right: 4, bottom: 6, left: 75 },
        w = width - margin.left - margin.right,
        h = (height + 200) - margin.top - margin.bottom;

    var x = d3.scale.ordinal()
           .rangeRoundBands([0, width / 1.5], .1)
    //.domain(response.map(function (d) {return d.industryName }));


    var y = d3.scale.linear()
        .range([height, 0])
    //.domain([0, d3.max(response, function (d) { return d.value })]);

    var chartSvg = d3.select(".bottomRight")
        .append("svg")
        .attr("width", width - 100)
        .attr("height", height + 100)
        .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")")

        .style("top", "20px");


    var xAxis = d3.svg.axis()
        .scale(x)
        .orient("bottom");


    var yAxis = d3.svg.axis()
        .scale(y)
        .orient("left")
        .ticks(10);




    //event functions 

    function clicked(d) {
        var x, y, k;
        $scope.areaName = d.properties.NAME10;
        var dataToSend = { areaName: $scope.areaName, year: $scope.selectedYear };
        if (d && centered !== d) {

            $http.post('Wages/data', dataToSend)
                .success(function (response) {
                    $scope.data = response;
                    updatePie(response);
                    updateColumns(response);
                })
                .error(function (response) { console.log(response); });

            var centroid = path.centroid(d);
            x = centroid[0];
            y = centroid[1];
            k = 4;
            centered = d;
        }
        else {
            $scope.areaName = "Statewide";
            dataToSend = { areaName: $scope.areaName, year: $scope.selectedYear };
            $http.post('Wages/data', dataToSend)
            .success(function (response) {
                $scope.data = response;
                updatePie(response);
                updateColumns(response);

            });

            x = width / 2;
            y = height / 2;
            k = 1;
            centered = null;

        }

        g.selectAll("path")
                .classed("active", centered && function (d) { return d === centered; });

        g.transition()
                .duration(750)
                .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")scale(" + k + ")translate(" + -x + "," + -y + ")")
                .style("stroke-width", 1.5 / k + "px");


    }

    //change data when Year is changed
    $scope.$watch("selectedYear", function (nv, ov) {

        if (nv !== ov) {
            var dataToSend = { areaName: $scope.areaName, year: $scope.selectedYear };
            $http.post('Wages/data', dataToSend)
                .success(function (response) {
                    $scope.data = response;
                    updatePie(response);
                    updateColumns(response);
                })
                .error(function (response) { console.log(response); });
        }

    });

    //pieChart update function

    function updatePie(response) {
        var totalEmp = 0;
        var pctEmp = d3.format(",.1%");
        for (var i = 0; i < response.length; i++) {
            totalEmp = totalEmp + response[i].value;


        }

        var color = d3.scale.category20();
        var path = pieSvg.selectAll("path")
            .data(pie(response));


        var text = pieSvg.selectAll("text")
            .data(pie(response))
            .attr("transform", function (d) {
                var c = arc.centroid(d),
                    x = c[0],
                    y = c[1],
                    h = Math.sqrt(x * x + y * y);
                return "translate(" + (x / h * (radius - 25)) + "," + (y / h * (radius - 25)) + ")";
            })
            .attr("dy", ".35em")
            .style("text-anchor", "middle")
            .attr('font-size', '10px')
            .text(function (d) { return pctEmp(d.data.value / totalEmp); });

        path.transition()
            .duration(500)
            .attr("fill", function (d, i) {
                return color(d.value);
            })
            .attr("d", arc)
            .each(function (d) {
                this._current = d;
            })
            .attrTween("d", arcTween);

        pieSvg.selectAll("title")
            .data(pie(response))
            .text(function (d) { return d.data.industryName; });
    };



    //pie chart initialization function
    function initialPie(response) {

        var totalEmp = 0;
        var pctEmp = d3.format(",.1%");
        for (var i = 0; i < response.length; i++) {
            totalEmp = totalEmp + response[i].value;


        }


        var color = d3.scale.category20();


        var path = pieSvg.selectAll("path")
            .data(pie(response))
            .enter()
            .append("path");

        var text = pieSvg.selectAll("text")
            .data(pie(response))
            .enter()
            .append("text")
            .attr("transform", function (d) {
                var c = arc.centroid(d),
                    x = c[0],
                    y = c[1],
                    h = Math.sqrt(x * x + y * y);
                return "translate(" + (x / h * (radius - 25)) + "," + (y / h * (radius - 25)) + ")";
            })
            .attr("dy", ".35em")
            .style("text-anchor", "middle")
            .attr('font-size', '10px')
            .text(function (d) { return pctEmp(d.data.value / totalEmp); });

        path.transition()
            .duration(500)
            .attr("fill", function (d, i) {
                return color(d.value);
            })
            .attr("d", arc)
            .each(function (d) {
                this._current = d;
            })
            .attrTween("d", arcTween);


        path.append("title")
            .text(function (d) { return d.data.industryName; });

    };

    //piechart service functions

    function arcTween(a) {
        var i = d3.interpolate(this._current, a);
        this._current = i(0);
        return function (t) {
            return arc(i(t));
        };
    }

    function midAngle(d) {
        return d.startAngle + (d.endAngle - d.startAngle) / 2;
    }


    //column chart functions

    function initialColumn(response) {

        x.domain(response.map(function (d) { return d.industryName }));
        y.domain([0, d3.max(response, function (d) { return d.value })]);

        chartSvg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis)
            .selectAll("text")
            .style("text-anchor", "end")
            .style("font-size", ".75em")
            .attr("dx", "-.8em")
            .attr("dy", ".15em")
            .attr("transform", function (d) {
                return "rotate(-65)"
            });

        chartSvg.append("g")
            .attr("class", "y axis")
            .call(yAxis)
            .append("text")
            .attr("transform", "rotate(-90)")
            .attr("dy", ".71em")
            .style("text-anchor", "end")
            .text("Wages");


        var bar = chartSvg.selectAll(".bar")
            .data(response)
            .enter()
            .append("rect")
            .attr("class", "bar")
            .attr("x", function (d) { return x(d.industryName); })
            .attr("width", x.rangeBand())
            .attr("y", function (d) { return y(d.value); })
            .attr("height", function (d) { return height - y(d.value); });
        //.attr("transform", function (d) { return "translate(" + x(d.industryName) + ",0)"; });

        bar.append("title")
            .text(function (d) { return d.industryName + ", " + d.value });

    }

    function updateColumns(response) {
        x.domain(response.map(function (d) { return d.industryName }));
        y.domain([0, d3.max(response, function (d) { return d.value })]);

        //var g = chartSvg.selectAll(".y");

        //g.selectAll("g")
        //    .call(yAxis);

        chartSvg.selectAll("g.y.axis").call(yAxis)

        var bar = chartSvg.selectAll(".bar")
            .data(response)
            .attr("class", "bar")
            .attr("x", function (d) { return x(d.industryName); })
            .attr("width", x.rangeBand())
            .attr("y", function (d) { return y(d.value); })
            .attr("height", function (d) { return height - y(d.value); });


        bar.transition()
            .duration(500);
        //.attr("transform", function (d) { return "translate(" + x(d.industryName) + ",0)"; });

        bar.append("title")
            .text(function (d) { return d.industryName + ", " + d.value });



    }





});

