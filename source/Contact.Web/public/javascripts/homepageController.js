var homepageController = function ($scope, $http, $timeout) {

    $scope.apiRoot = "http://milescontact.cloudapp.net";

    $scope.queryTerm = "";

    $scope.selectedEmployee = null;

    $scope.search = function() {
        var tmpTerm = $scope.queryTerm;

        if (!tmpTerm) {
            $scope.searchResult = null;
            return;
        }

        $timeout(function() {
            if (tmpTerm == $scope.queryTerm) {
                var res = $http({
                    method: 'GET',
                    url: $scope.apiRoot + "/api/Search?query=" + encodeURIComponent(tmpTerm),
                    withCredentials: true
                });

                res.success(function(data) {
                    if (tmpTerm == $scope.queryTerm) {
                        $scope.searchResult = data;
                    }
                });
            }
        }, 250);
    };

    $scope.checkAuthenticated = function() {
        var res = $http({
            method: 'GET',
            url: $scope.apiRoot + "/api/test",
            withCredentials: true
        });
        res.success(function (data) {

            $scope.isAuthenticated = { good: true, name: data.replace(/"/g, "") };
            $('#mainContent').show();
        });
        res.error(function(errorData,status) {
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

    var getEnglishDetails = function(details) {

        var groups = groupBy(details.Competency, function(item) {
            return [item.InternationalCategory];
        });

        var description = details.Descriptions;
        if (description.length > 0) {
            description = description[0].InternationalDescription;
        }
        for (var index = 0; index < groups.length; ++index) {
            var group = groups[index];
            group.key = group.key.replace('["', '').replace('"]', '');

            for (var comp = 0; comp < group.competencies.length; comp++) {
                var competency = group.competencies[comp];
                competency.competency = competency.InternationalCompentency;
                delete competency.InternationalCompentency;
                delete competency.Category;
                delete competency.InternationalCategory;
                delete competency.Competency;
            }
        }
        return {
            description: description,
            competencygroups: groups
        }
    }

    var getLocalDetails = function (details) {
        var groups = groupBy(details.Competency, function (item) {
            
            return [item.Category];
        });

        var description = details.Descriptions;
        if (description.length > 0) {
            description = description[0].LocalDescription;
        }
        for (var index = 0; index < groups.length; ++index) {
            var group = groups[index];
            group.key = group.key.replace('["', '').replace('"]', '');

            for (var comp = 0; comp < group.competencies.length; comp++) {
                var competency = group.competencies[comp];
                competency.competency = competency.Competency;
                delete competency.InternationalCompentency;
                delete competency.Category;
                delete competency.InternationalCategory;
                delete competency.Competency;
            }
        }
        return {
            description: description,
            competencygroups: groups
        }
    }

    function groupBy( array , f )
    {
        var groups = {};
        array.forEach( function( o )
        {
            var group = JSON.stringify( f(o) );
            groups[group] = groups[group] || [];
            groups[group].push(o);
            if (!groups[group].key) {
                groups[group].key = group;
            }
        });
        return Object.keys(groups).map(function(group) {
            return { key: groups[group].key, competencies: groups[group] };
        });
    }


    angular.element(document).ready(function() {
        $scope.checkAuthenticated();
    });

}