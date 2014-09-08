var homepageController = function ($scope, $http, $timeout) {

    $scope.apiRoot = "http://milescontact.cloudapp.net";

    $scope.queryTerm = "";

    $scope.selectedEmployee = {Name:"test"};
   
    $scope.queryTerm = "";

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
                    alert(errorData);
                }
            }
        );
    }
    $scope.showDetails = function(item) {
        $scope.selectedEmployee = { Name: item.Name };
    }

    angular.element(document).ready(function() {
        $scope.checkAuthenticated();
    });

}