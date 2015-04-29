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

});

