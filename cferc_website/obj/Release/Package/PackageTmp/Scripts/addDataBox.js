function showWindow() {
    $('.addDataBox').stop().animate({ 'marginTop': '0px' }, 500);
}

function hideWindow() {
    $('#addDataPart2').stop().animate({ 'marginLeft': '-2500px' }, 500);
    $('.addDataBox').stop().animate({ 'marginTop': '700px' }, 500);
}

function nextPage() {
    $('#addDataPart2').stop().animate({ 'marginLeft': '0px' }, 500);
}

function previousPage() {
    $('#addDataPart2').stop().animate({ 'marginLeft': '-2500px' }, 500);
}




var app = angular.module("app", [])
//factory definition
app.factory('dataFactory', function ($http, $q) {

    var service = {};
    service.getArea = function () {
        var deferred = $q.defer();
        $http.get('add/getAreas')
            .success(function(response){
                deferred.resolve(response);
            }).error(function (response) {
                deferred.reject('There was an error processing GET request');
            })
        return deferred.promise;
    }

    service.postArea = function (dataToSend) {
        var deferred = $q.defer();
        $http.post('add/postArea', dataToSend)
            .success(function(response){
                deferred.resolve(response);
            }).error(function(response){
                deferred.reject('There was an error processing POST request');
            })

        return deferred.promise;
    }

    service.getindustry = function () {
        var deferred = $q.defer();
        $http.get('add/getindustries')
            .success(function (response) {
                deferred.resolve(response);
            }).error(function (response) {
                deferred.reject('There was an error processing GET request');
            })
        return deferred.promise;
    }

    service.postindustry = function (dataToSend) {
        var deferred = $q.defer();
        $http.post('add/postindustry', dataToSend)
            .success(function (response) {
                deferred.resolve(response);
            }).error(function (response) {
                deferred.reject('There was an error processing POST request');
            })

        return deferred.promise;
    }

    service.getmeasure = function () {
        var deferred = $q.defer();
        $http.get('add/getmeasures')
            .success(function (response) {
                deferred.resolve(response);
            }).error(function (response) {
                deferred.reject('There was an error processing GET request');
            })
        return deferred.promise;
    }

    service.postmeasure = function (dataToSend) {
        var deferred = $q.defer();
        $http.post('add/postmeasure', dataToSend)
            .success(function (response) {
                deferred.resolve(response);
            }).error(function (response) {
                deferred.reject('There was an error processing POST request');
            })

        return deferred.promise;
    }

    service.postData = function (dataToSend) {
        var deferred = $q.defer();
        $http.post('add/postdata', dataToSend)
            .success(function (response) {
                deferred.resolve(response);
            }).error(function (response) {
                deferred.reject('There was an error processing POST request');
            })

        return deferred.promise;
    }

    return service;

});


app.controller("addData", function ($scope, $compile, dataFactory) {
    //initialize boolean disabled variables
    $scope.areaIDDisabled = false;
    $scope.areaNameDisabled = false;
    $scope.areaList = [];
    $scope.selectedArea;
    $scope.inputPeriod = [];
    $scope.inputValue = [];


    
    var colCount = 1;
    var rowCount = 2;
    var dataArray = [];


    function addColumn() {
        //add period label        
        d3.select('#col' + rowCount + colCount).append("label")
        .attr("class", "FieldLabel")
        .attr("for", "periodLabel" + rowCount + colCount)
        .attr("id", "periodLabel" + rowCount + colCount)
        .text("Period:");
        //add period input
        var period = d3.select('#col' + rowCount + colCount).append("input")
        .attr("type", "text")
        .attr("class", "form-control inputField")
        .attr("id", "periodInput" + rowCount + colCount)
        .attr("ng-model", "period" + rowCount + colCount);
        //.attr("ng-model", "newData.period")
        //.attr("ng-model-options", "{getterSetter: true}")
        //add value label
        d3.select('#col' + rowCount + colCount).append("label")
        .attr("class", "FieldLabel")
        .attr("for", "valueInput" + rowCount + colCount)
        .attr("id", "valueLabel" + rowCount + colCount)
        .text("Value:");
        //add value input
        var input = d3.select('#col' + rowCount + colCount).append("input")
        .attr("type", "text")
        .attr("class", "form-control inputField")
        .attr("id", "valueInput" + rowCount + colCount)
        .attr("ng-model", "value" + rowCount + colCount);
        //.attr("ng-model", "newData.value")
        //.attr("ng-model-options", "{getterSetter: true}")

        //compile DOM so that the dynamically added elements are bound to the scope variables
        $compile(period[0][0])($scope);
        $compile(input[0][0])($scope);
        


    }

    function addRow() {
        d3.select('#page2Container').append("div")
        .attr("id", "row" + rowCount)
        .attr("class", "row");


        d3.select('#row' + rowCount).append("div")
        .attr("class", "col-md-2 col")
        .attr("id", "col" + rowCount + "1");

        d3.select('#row' + rowCount).append("div")
       .attr("class", "col-md-2 col" + rowCount + "2")
        .attr("id", "col" + rowCount + "2");

        d3.select('#row' + rowCount).append("div")
       .attr("class", "col-md-2 col" + rowCount + "3")
        .attr("id", "col" + rowCount + "3");

        d3.select('#row' + rowCount).append("div")
       .attr("class", "col-md-2 col" + rowCount + "4")
        .attr("id", "col" + rowCount + "4");

        d3.select('#row' + rowCount).append("div")
       .attr("class", "col-md-2 col" + rowCount + "5")
        .attr("id", "col" + rowCount + "5");

        d3.select('#row' + rowCount).append("div")
       .attr("class", "col-md-2 col" + rowCount + "6")
        .attr("id", "col" + rowCount + "6");



    }

    function removeColumn(row, col) {
        d3.select('#periodLabel' + row + col).remove();
        var period = d3.select('#periodInput' + row + col).remove();
        d3.select('#valueLabel' + row + col).remove();
        var input = d3.select('#valueInput' + row + col).remove();
        $compile(period[0][0])($scope);
        $compile(input[0][0])($scope);

    }

    function add() {
        colCount++;
        
        if (colCount < 7) {
            addColumn();

        }
        else {
            colCount = 1;
            rowCount++;
            addRow();
            addColumn();

        }
        
    }


    $scope.addPeriod = function () {
        add();
        $scope.col = colCount;
        $scope.row = rowCount;
        
    };

    $scope.remove = function () {
        
        if (colCount != 1) {
            removeColumn(rowCount, colCount);
            colCount--;
           
        }        
        else if (colCount == 1 && rowCount !=2) {
            removeColumn(rowCount, colCount);
            rowCount--;
            colCount = 6;            
           
        }

    }

    
    //event to watch for selected area
    $scope.$watch("selectedArea", function (nv, ov) {
        if (!$scope.selectedArea) {
            $scope.areaIDDisabled = false;
            $scope.areaNameDisabled = false;
        }
        else {
            $scope.areaIDDisabled = true;
            $scope.areaNameDisabled = true;
        }
        
    });

    //populate select boxes with values

    
    $scope.fillAreaSelect = function () {
        
        dataFactory.getArea()
        .then(function (response) {
           $scope.areaList = response; 
        }, function (response) {
            alert(response);
        });
    }
    $scope.fillAreaSelect();

    
    //post Functions
    
    $scope.postArea = function () {
        dataToSend = { areaID: $scope.newAreaId, areaName: $scope.newAreaName }
        dataFactory.postArea(dataToSend)
        .then(function (response) {
            alert("The area: " + response.areaName + " has been added to the database");            
            $scope.areaList.push(response)
            $scope.selectedArea = $scope.areaList[$scope.areaList.length - 1];
            $scope.newAreaId = null;
            $scope.newAreaName = null;
        }, function (response) {
            alert("The area: " + dataToSend.areaName + "already exists in the database");
        });
    }

    //////////////////industry functions////////////////////////
    //initialize boolean disabled variables
    $scope.industryIDDisabled = false;
    $scope.industryNameDisabled = false;
    $scope.industryList = [];
    $scope.selectedindustry;



    //event to watch for selected industry
    $scope.$watch("selectedindustry", function (nv, ov) {
        if (!$scope.selectedindustry) {
            $scope.industryIDDisabled = false;
            $scope.industryNameDisabled = false;
        }
        else {
            $scope.industryIDDisabled = true;
            $scope.industryNameDisabled = true;
        }

    });

    //populate select boxes with values


    $scope.fillindustrySelect = function () {

        dataFactory.getindustry()
        .then(function (response) {
            $scope.industryList = response;            
        }, function (response) {
            alert(response);
        });
    }
    $scope.fillindustrySelect();


    //post Functions

    $scope.postindustry = function () {
        dataToSend = { industryID: $scope.newindustryId, industryName: $scope.newindustryName }
        dataFactory.postindustry(dataToSend)
        .then(function (response) {
            alert("The industry: " + response.industryName + " has been added to the database");
            $scope.industryList.push(response)
            $scope.selectedindustry = $scope.industryList[$scope.industryList.length - 1];
            $scope.newindustryId = null;
            $scope.newindustryName = null;
        }, function (response) {
            alert("The industry: " + dataToSend.industryName + "already exists in the database");
        });
    }


    //////measure functions////////
    //initialize boolean disabled variables
    $scope.measureIDDisabled = false;
    $scope.measureNameDisabled = false;
    $scope.measureList = [];
    $scope.selectedmeasure;



    //event to watch for selected measure
    $scope.$watch("selectedmeasure", function (nv, ov) {
        if (!$scope.selectedmeasure) {
            $scope.measureIDDisabled = false;
            $scope.measureNameDisabled = false;
        }
        else {
            $scope.measureIDDisabled = true;
            $scope.measureNameDisabled = true;
        }

    });

    //populate select boxes with values


    $scope.fillmeasureSelect = function () {

        dataFactory.getmeasure()
        .then(function (response) {
            $scope.measureList = response;
        }, function (response) {
            alert(response);
        });
    }
    $scope.fillmeasureSelect();


    //post Functions

    $scope.postmeasure = function () {
        dataToSend = { measureID: $scope.newmeasureId, measureName: $scope.newmeasureName }
        dataFactory.postmeasure(dataToSend)
        .then(function (response) {
            alert("The measure: " + response.measureName + " has been added to the database");
            $scope.measureList.push(response)
            $scope.selectedmeasure = $scope.measureList[$scope.measureList.length - 1];
            $scope.newmeasureId = null;
            $scope.newmeasureName = null;
        }, function (response) {
            alert("The measure: " + dataToSend.measureName + "already exists in the database");
        });
    }

    //post data

    $scope.submitData = function () {
        //build data array to send to server
        for (var i = 1; i < rowCount + 1; i++) {
            for (var j = 1; j < 13; j++) {
                if (angular.isDefined($scope['period' + i + j])) {
                    var dataObj = {
                        year: $scope['period' + i + j],
                        value: $scope['value' + i + j],
                        seriesID: $scope.selectedArea.areaID + $scope.newTableName,
                        period: "NA"
                    }
                    dataArray.push(dataObj);
                }
            }
        }




        var dataToSend = {

            seriesID: $scope.selectedArea.areaID + $scope.newTableName,
            areaID: $scope.selectedArea.areaID,
            measureID: $scope.selectedmeasure.measureID,
            industryID: $scope.selectedindustry.industryID,
            blsTable: $scope.newTableName,
            beginYear: dataArray[0].year,
            endYear: dataArray[dataArray.length - 1].year,
            beginPeriod: "NA",
            endPeriod: "NA",
            data_table: dataArray
        };


        dataFactory.postData(dataToSend)
        .then(function (response) {
            alert("Series was successfully added");
        }, function (response) {
            alert("There was an error updating database");
        });
        console.log(dataToSend);

        


    }
});

