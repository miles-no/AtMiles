(function(module) {
     var SearchController = function ($scope, $http, $timeout, $location, auth, store, $rootScope, DataFactory) {

        //redirect to login page if not authenticated
        if($rootScope.isAuthenticated == false) {
            $location.path('/login');
        }

        $scope.apiRoot = "https://api-at.miles.no";

        $scope.queryTerm = "";
        $scope.$watch('queryTerm', function() {
            $scope.search(false);
        });


        $scope.searching = false;  //monitor for making search execute in one instance

        var getCompany = function () {
            // Used to divide into companies. Useless just now
            return '';
            //var p = $location.path().split("/");
            //return p[1] || "Unknown";
        }
        var lastQueryTerm = null;
        var maxSearchResults = 9;

        var skip = 0;
        var take = maxSearchResults;

        $scope.selectedEmployee = null;
        $scope.dataLoading = false; //used to detect when we're waiting for data

        $scope.searchPerformed = false;
        $scope.moreSearchResults = false;
        $scope.searchResult = { Results: [], Skipped: 0, Total: 0 };

        $(window).resize(function() {
            $scope.$apply(function() {
                calculateSearchFieldWidth();
            });
        });

        $scope.$watch('isAuthenticated', function(newVal, oldVal) {
            setTimeout(function(){
                $scope.$apply(function (){
                    calculateSearchFieldWidth();
                });
            },20);
        });

        function calculateSearchFieldWidth() {
            var searchContainer = $('#searchContainer');
            $scope.searchWidth =  searchContainer.width() - $('#searchButton').width() - 30;
        }

        $scope.search = function (add) {
            //mointor to avoid multiple searches starting simultaneously
            //this was needed after introducing infinite-scroll
            if($scope.searching) {
                return;
            }
            $scope.searching = true;

            if(add && $scope.moreSearchResults == false) {
                $scope.searching = false;
                return;
            }

            if ($scope.queryTerm == lastQueryTerm && !add) {
                $scope.searching = false;
                return;
            }
            var tmpTerm = $scope.queryTerm;

            if (!add) {
                skip = 0;
            }

            $timeout(function () {
                if (tmpTerm == $scope.queryTerm) {
                    $scope.dataLoading = true;
                    var res = DataFactory.search($scope.apiRoot, tmpTerm,take, skip);

                    res.success(function (data) {
                        $scope.dataLoading = false;

                        if (data.Error) {
                            Messenger().post({
                                message: 'Error retrieving data: ' + data.Error,
                                type: 'error',
                                showCloseButton: true
                            });
                            $scope.searching = false;
                            return;
                        }
                        if (tmpTerm == $scope.queryTerm) {
                            if (data) {
                                if (!add) {
                                    while ($scope.searchResult.Results.length > 0) {
                                        $scope.searchResult.Results.pop();
                                    }
                                    $scope.selectedEmployee = null;
                                    $scope.totalHits = data.Total;
                                    $scope.searchPerformed = true;
                                }

                                for (var i = 0; i < data.Results.length; i++) {
                                    $scope.searchResult.Results.push(data.Results[i]);
                                }

                                if (data.Total > $scope.searchResult.Results.length) {
                                    skip = data.Skipped + data.Results.length;
                                    $scope.moreSearchResults = data.Total - skip;
                                } else {
                                    skip = 0;
                                    $scope.moreSearchResults = false;
                                }
                                lastQueryTerm = tmpTerm;
                                //Disabled until we want to display a person
                                //if ($scope.searchResult.Results.length == 1) {
                                //    $scope.showDetails($scope.searchResult.Results[0]);
                                //}
                                $scope.searching = false;
                            }
                            else {
                                $scope.searching = false;
                            }
                         }
                        else {
                            $scope.searching = false;
                        }
                    }).error( function(data, status, headers, config) {
                        console.log("Error retrieving data:");
                        console.log("data:");
                        console.log(data);
                        console.log("status:");
                        console.log(status);
                        console.log("headers:");
                        console.log(headers);
                        console.log("config:");
                        console.log(config);
                        $scope.dataLoading = false;
                        $timeout(function(){
                            $scope.searching = false;
                        }, 5000);
                        Messenger().post({
                            message: 'Failed to retrieve data. Please try again. Check console for errors and report back',
                            type: 'error',
                            showCloseButton: true
                        });
                    });
                }
                else {
                    $scope.searching = false;
                }
             }, 250);
        };


        $scope.showDetails = function (item) {

            var res = $http({
                method: 'GET',
                //TODO: fix getCompany to avoid hard-coding
                //url: $scope.apiRoot + "api/company/"+ getCompany() + "/employee/" + item.GlobalId,
                url: $scope.apiRoot + "/api/company/miles/employee/" + item.GlobalId,
                withCredentials: true
            });
            res.success(function (data) {

                $scope.selectedEmployee = {
                    name: data.Name,
                    title: data.JobTitle,
                    phoneNumber: data.PhoneNumber,
                    address : data.Address,
                    email: data.Email,
                    english: getEnglishDetails(data),
                    local: getLocalDetails(data),
                    canEdit: true//data.CanEdit
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

            for (var i = 0; i < groups.length; i++) {
                groups[i].competencies = cleanArray(groups[i].competencies);
                if (groups[i].competencies.length == 0) {
                    groups.splice(i, 1);
                    i = i - 1;
                }
            }
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

            for (var i = 0; i < groups.length; i++) {
                groups[i].competencies = cleanArray(groups[i].competencies);
                if (groups[i].competencies.length == 0) {
                    groups.splice(i, 1);
                    i = i - 1;
                }
            }

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
                if (o) {
                    groups[group].push(o);
                }

                if (!groups[group].key) {
                    groups[group].key = group;
                }
            });
            return Object.keys(groups).map(function (group) {
                return { key: groups[group].key.replace('["', '').replace('"]', ''), competencies: project(groups[group]) };
            });
        }

        function cleanArray(actual){
            var newArray = new Array();
            for(var i = 0; i<actual.length; i++){
                if (actual[i]){
                    newArray.push(actual[i]);
                }
            }
            return newArray;
        }

        angular.element(document).ready(function () {
            calculateSearchFieldWidth();
        });
     };

    module.controller('SearchController', SearchController);

}(angular.module('AtMiles')));
