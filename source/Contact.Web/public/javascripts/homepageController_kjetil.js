var homepageController = function ($scope, $http, $timeout, $location, auth) {

    $scope.apiRoot = "http://milescontact.cloudapp.net/";
   

    $scope.queryTerm = "";

    $scope.errors = [];
    
    $scope.isAuthenticated = false;
    $scope.userNeedToFixBusyTime = false;  //used to signal GUI to display users busy time
    $scope.busyTime = null;                //users busy time
    $scope.busyTimeEditRow = -1;           //if user edits a row, this holds which one
    $scope.debugMessage = "";
    
    $scope.login = function () {
        
        auth.signin({
            popup: true
        }, function () {
            $scope.isAuthenticated = true;
            checkUserBusyTime();
        }, function () {
            $scope.isAuthenticated = false;
        });
    }

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

    $scope.moreSearchResults = false;
    $scope.searchResult = { Results: [], Skipped: 0, Total: 0 };

    $scope.search = function (add) {
        if ($scope.queryTerm == lastQueryTerm && !add) {
            return;
        }
        var tmpTerm = $scope.queryTerm;

        if (!tmpTerm || tmpTerm == '') {
            $scope.selectedEmployee = null;
            while ($scope.searchResult.Results.length > 0) {
                skip = 0;
                $scope.searchResult.Results.pop();
            }
           
            return;
        }
        if (!add) {
            skip = 0;
        }

        $timeout(function () {
            if (tmpTerm == $scope.queryTerm) {
                var res = $http({
                    method: 'GET',
                    url: $scope.apiRoot + getCompany() +"/api/Search/fulltext?take=" + take + "&skip=" + skip + "&query=" + encodeURIComponent(tmpTerm),
                    withCredentials: true
                });

                res.success(function (data) {
                    if (data.Error) {
                        $scope.errors.push(data.Error);
                        return;
                    }
                    if (tmpTerm == $scope.queryTerm) {
                        if (data) {
                            if (!add) {
                                while ($scope.searchResult.Results.length > 0) {
                                    $scope.searchResult.Results.pop();
                                }
                                $scope.selectedEmployee = null;
                            }

                            for (var i = 0; i < data.Results.length; i++) {
                                $scope.searchResult.Results.push(data.Results[i]);
                            }

                            if (data.Total > $scope.searchResult.Results.length) {
                                skip = data.Skipped + data.Results.length;
                                $scope.moreSearchResults = data.TotalResults - skip;
                            } else {
                                skip = 0;
                                $scope.moreSearchResults = false;
                            }
                            lastQueryTerm = tmpTerm;
                            if ($scope.searchResult.Results.length == 1) {
                                $scope.showDetails($scope.searchResult.Results[0]);
                            }
                        }
                    }
                });
            }
        }, 250);
    };

  
    $scope.showDetails = function (item) {

        var res = $http({
            method: 'GET',
            //TODO: fix getCompany to avoid hard-coding
            //url: $scope.apiRoot + "api/company/"+ getCompany() + "/employee/" + item.GlobalId,
            url: $scope.apiRoot + "api/company/miles/employee/" + item.GlobalId,
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
                canEdit: true, //data.CanEdit
                busyTime: data.BusyTimeEntries
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

    //function to retrieve users busy time
    function checkUserBusyTime() {
        $scope.userBusyTime = null;

        $scope.busyTime = {
            "expieryDate": "2014-10-20T00:00:00",
            "busyTimeEntries": [
                {"Id": "CBKckKLdBEKfoPSfxJcWEw", "Start": "2014-10-20T00:00:00", "End": "2014-11-25T00:00:00", "PercentageOccupied": 100, "Comment": "Aker Geo"},
                {"Id": "xLoqaDiOm0mFHGmoMT7Pgw", "Start": "2014-11-26T00:00:00", "End": "2015-02-28T00:00:00", "PercentageOccupied": 50, "Comment": "Egenlæring"}
            ]
        };



        $scope.userNeedToFixBusyTime = true;
    }

    //called when users is done verifying busy time
    $scope.busyTimeAssignmentVerified = function() {
        //TODO notify server
        $scope.userNeedToFixBusyTime = false;
    };

    //called when user deletes a specific row
    $scope.deleteBusyTimeAssignment = function(busyTimeEntry) {
        $scope.debugMessage = "Delete "+ busyTimeEntry.Id;
        //TODO notify server
    };

    $scope.editBusyTime = function(row) {
        $scope.busyTimeEditRow = row;
    }

    angular.element(document).ready(function () {
       
    });

   
}

var atMiles = angular.module('AtMiles', ['auth0']).config(function ($locationProvider, $httpProvider, authProvider) {

    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    });
    authProvider.init({
        domain: 'atmiles.auth0.com',
        clientID: '6jsWdVCPDiKSdSKi2n7naqmy7eeO703H',
        callbackURL: location.href
    });
    $httpProvider.interceptors.push('authInterceptor');
})

.run(function (auth) {
    // This hooks al auth events to check everything as soon as the app starts
    auth.hookEvents();
});


atMiles.controller('homepageController', ['$scope', '$http', '$timeout','$location','auth', homepageController]);
