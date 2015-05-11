var app = angular.module("app", ['cgBusy']);
app.factory('downloadFactory', function ($http, $q) {

    var service = {};

    service.getMeasures = function () {

        var deferred = $q.defer();
        $http.get('download/getMeasure')
            .success(function(response){
                deferred.resolve(response);
            })
            .error(function(response){
                deferred.reject('There was an error processing the get request')
            })
        return deferred.promise;
    }

    service.industryRequest = function (dataToSend) {

        var deferred = $q.defer();
        $http.post('download/getIndustry', dataToSend)
            .success(function(response){
                deferred.resolve(response);
            })
            .error(function(response){
                deferred.reject('There was an error processing the request')
            })
        return deferred.promise;

    }

    service.areaRequest = function (dataToSend) {

        var deferred = $q.defer();
        $http.post('download/getArea', dataToSend)
            .success(function (response) {
                deferred.resolve(response);
            })
            .error(function (response) {
                deferred.reject('There was an error processing the request')
            })
        return deferred.promise;

    }

    service.yearRequest = function (dataToSend) {

        var deferred = $q.defer();
        $http.post('download/getYears', dataToSend)
            .success(function (response) {
                deferred.resolve(response);
            })
            .error(function (response) {
                deferred.reject('There was an error processing the request')
            })
        return deferred.promise;

    }

    service.dataRequest = function (dataToSend) {

        var deferred = $q.defer();
        $http.post('download/getData', dataToSend)
            .success(function (response) {
                deferred.resolve(response);
            })
            .error(function (response) {
                deferred.reject('There was an error processing the request')
            })
        return deferred.promise;

    }

    service.dataDownload = function (dataToSend) {

        var deferred = $q.defer();
        $http.post('download/downloadCsv', dataToSend)
            .success(function (response) {
                deferred.resolve(response);
            })
            .error(function (response) {
                deferred.reject('There was an error processing the request')
            })
        return deferred.promise;

    }

    return service;

});

app.controller('download', function ($scope, downloadFactory) {


    //variable definition
    $scope.selectedMeasure;
    $scope.selectedIndustry;
    $scope.selectedArea;
    $scope.beginYear;
    $scope.endYear;
    $scope.output = [];
    


    //function definition
    $scope.fillMeasureSelect = function () {

        downloadFactory.getMeasures()
        .then(function (response) {
            $scope.measureList = response;
           
        }, function (response) {
            alert(response);
        });
    }



    $scope.fillMeasureSelect();

    $scope.fillIndustrySelect = function () {
        var dataToSend = {
            measureID: $scope.selectedMeasure.measureID,
            measureName: $scope.selectedMeasure.measureName
        };

        $scope.myPromise = downloadFactory.industryRequest(dataToSend)
        .then(function (response) {
            $scope.industryList = response;
        }, function (response) {
            alert(response);
        });
    }

    $scope.fillAreaSelect = function () {
        var dataToSend = {
            measureID: $scope.selectedMeasure.measureID,
            measureName: $scope.selectedMeasure.measureName,
            industryID: $scope.selectedIndustry.industryID,
            industryName: $scope.selectedIndustry.industryName
        };

        $scope.myPromise = downloadFactory.areaRequest(dataToSend)
        .then(function (response) {
            $scope.areaList = response;
        }, function (response) {
            alert(response);
        });
    }

    $scope.fillYearSelect = function () {
        var dataToSend = {
            measureID: $scope.selectedMeasure.measureID,
            measureName: $scope.selectedMeasure.measureName,
            industryID: $scope.selectedIndustry.industryID,
            industryName: $scope.selectedIndustry.industryName,
            area: $scope.selectedArea
        };
        console.log(dataToSend);
        $scope.myPromise = downloadFactory.yearRequest(dataToSend)
        .then(function (response) {
            $scope.yearList = response;
            
            
            for (var i = parseInt($scope.yearList[0].beginYear); i < parseInt($scope.yearList[0].endYear) +1 ; i++) {
                $scope.output.push(i)
            }
            

        }, function (response) {
            alert(response);
        });
    }


    $scope.getData = function () {
        var dataToSend = {
            measureID: $scope.selectedMeasure.measureID,
            measureName: $scope.selectedMeasure.measureName,
            industryID: $scope.selectedIndustry.industryID,
            industryName: $scope.selectedIndustry.industryName,
            area: $scope.selectedArea,
            beginYear: $scope.selectedBeginYear,
            endYear: $scope.selectedEndYear
        };

        $scope.myPromise = downloadFactory.dataRequest(dataToSend)
        .then(function (response) {
            $scope.data = response;
            //console.log(response);
            createGraph(response);
        }, function (response) {
            alert(response);
        });
    }


    

    //watch functions
    $scope.$watch('selectedMeasure', function (nv, ov) {
        if (nv != ov) {
            $scope.fillIndustrySelect();
        }
    });

    $scope.$watch('selectedIndustry', function (nv, ov) {
        if (nv != ov) {
            $scope.fillAreaSelect();
        }
    });

    $scope.$watch('selectedArea', function (nv, ov) {
        if (nv != ov) {
            $scope.fillYearSelect();
            $scope.output = [];
            console.log($scope.selectedArea);
        }
    });

    $scope.$watch('selectedEndYear', function (nv, ov) {

        if (nv !== ov) {
            var beginYear = parseInt($scope.selectedBeginYear);
            var endYear = parseInt($scope.selectedEndYear);
            if (endYear < beginYear) {
                alert('Error! The series end year must be greater than the series beginning year')
                document.getElementById('queryBtn').disabled = true;
            }
            else {
                document.getElementById('queryBtn').disabled = false;
            }
        }
    });
    var margin = { top: 4, right: 4, bottom: 4, left: 4 }
    h = 350 - margin.top - margin.bottom,
    w = 650 - margin.left - margin.right;

    var svg = d3.select("#graph1").append("svg")
    .attr("height", h)
    .attr("width", w)
    .append("g")
    .attr("class", "mainG")
    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");


    //d3 charts

    function createGraph(response) {

        svg.selectAll("path").remove();
        svg.selectAll(".axis").remove();

        var tickNo = ($scope.selectedEndYear - $scope.selectedBeginYear) + 1;
        var data = d3.nest()
            .key(function (d) { return d.areaName; })
            .entries(response)

        console.log(data);


            
        var color = d3.scale.category10();

        var x = d3.scale.linear()
            .range([0, w - 50]);

        var y = d3.scale.linear()
            .range([h,0]);


        var xAxis = d3.svg.axis()
            .scale(x)
            .orient("bottom")
            .ticks(tickNo)
            .tickFormat(d3.format(""));


        var yAxis = d3.svg.axis()
            .scale(y)
            .orient("left");

        var line = d3.svg.line()
            .x(function (d) { return x(d.year); })
            .y(function (d) { return y(d.val); });




        var xAxisSel = svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + (h - 35) + ")");
            

        var yAxisSel = svg.append("g")
               .attr("class", "y axis");
                
            
        data.forEach(function (obj, index, arr) {
            //gSeries = d3.select(".mainG").selectAll(".seriesContainer")
            //    .data(obj)
            //    .enter()
            //    .append("g");
            var eachObj = obj;
            var valArray = [];
            eachObj.values.forEach(function (i) { valArray.push(i.val); });

            x.domain(d3.extent(obj.values, function (d) { return d.year; }));
            y.domain(d3.extent(valArray, function (d) { return d; }));
            
        });

        yAxisSel.call(yAxis);
        xAxisSel.call(xAxis);

        data.forEach(function (obj, index, arr) {

            var eachObj = obj;
            var valArray = [];
            eachObj.values.forEach(function (i) { valArray.push(i.val); });

            x.domain(d3.extent(obj.values, function (d) { return d.year; }));
            y.domain(d3.extent(valArray, function (d) { return d; }));

            svg
                .append("path")
                .datum(obj.values)
                .attr("class", "line").transition().duration(500)
                .attr("d", line);
                
                
        });
            
            

            

    }


});

