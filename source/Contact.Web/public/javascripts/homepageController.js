var homepageController = function ($scope, $http, $timeout) {

    $scope.apiRoot = "http://milescontact.cloudapp.net";

    $scope.queryTerm = "";
    var lastQueryTerm = null;

    $scope.selectedEmployee = null;

    $scope.searchResult = { Results: [], Skipped: 0, Total: 0 };

    $scope.search = function () {
        if ($scope.queryTerm == lastQueryTerm) {
            return;
        }
        var tmpTerm = $scope.queryTerm;

        if (!tmpTerm || tmpTerm == '') {
            while ($scope.searchResult.Results.length > 0) {
                $scope.searchResult.Results.pop();
            }
           
            return;
        }

        $timeout(function () {
            if (tmpTerm == $scope.queryTerm) {
                var res = $http({
                    method: 'GET',
                    url: $scope.apiRoot + "/api/Search?query=" + encodeURIComponent(tmpTerm),
                    withCredentials: true
                });

                res.success(function (data) {
                    if (tmpTerm == $scope.queryTerm) {
                        if (data != null) {
                            //$scope.searchResult.Total = data.Total;
                            //$scope.searchResult.Skipped = data.Skipped;

                            while ($scope.searchResult.Results.length > 0) {
                                $scope.searchResult.Results.pop();
                            }
                            

                            for (var i = 0; i < data.Results.length; i++) {
                                $scope.searchResult.Results.push(data.Results[i]);
                            }

                            lastQueryTerm = tmpTerm;
                        }
                    }
                });
            }
        }, 250);
    };

    $scope.checkAuthenticated = function () {
        var res = $http({
            method: 'GET',
            url: $scope.apiRoot + "/api/test",
            withCredentials: true
        });
        res.success(function (data) {

            $scope.isAuthenticated = { good: true, name: data.replace(/"/g, "") };
            $('#mainContent').show();
        });
        res.error(function (errorData, status) {
            if (status == 401) {
                $scope.isAuthenticated = { good: false, name: "unknown" };
                $('#mainContent').hide();
            } else {
                alert(errorData.data);
            }
        }
        );
    }
    $scope.showDetails = function (item) {

        var res = $http({
            method: 'GET',
            url: $scope.apiRoot + "/api/company/miles/employee/" + item.GlobalId,
            withCredentials: true
        });
        res.success(function (data) {

            $scope.selectedEmployee = {
                name: data.Name,
                title: data.JobTitle,
                phoneNumber: data.PhoneNumber,
                email: data.Email,
                english: getEnglishDetails(data),
                local: getLocalDetails(data),
            };
        });
    }

    var getEnglishDetails = function (details) {

        var groups = groupBy(details.Competency,
            function (item) {
                return [item.InternationalCategory];
            },
            function (array) {
                return array.map(function (c) {
                    return c.InternationalCompentency;
                });
            });

        var description = details.Descriptions;
        if (description.length > 0) {
            description = description[0].InternationalDescription;
        }

        return {
            description: description,
            competencygroups: groups
        }
    }

    var getLocalDetails = function (details) {

        var groups = groupBy(details.Competency,
            function (item) {
                return [item.Category];
            },
            function (array) {
                return array.map(function (c) {
                    return c.Competency;
                });
            });

        var description = details.Descriptions;
        if (description.length > 0) {
            description = description[0].LocalDescription;
        }

        return {
            description: description,
            competencygroups: groups
        }
    }

    function groupBy(array, f, project) {
        var groups = {};
        array.forEach(function (o) {
            var group = JSON.stringify(f(o));
            groups[group] = groups[group] || [];
            groups[group].push(o);
            if (!groups[group].key) {
                groups[group].key = group;
            }
        });
        return Object.keys(groups).map(function (group) {
            return { key: groups[group].key.replace('["', '').replace('"]', ''), competencies: project(groups[group]) };
        });
    }


    angular.element(document).ready(function () {
        $scope.checkAuthenticated();
    });

}

var atMiles = angular.module('AtMiles', ['ngAnimate']);
atMiles.controller('homepageController', ['$scope', '$http', '$timeout', homepageController]);
