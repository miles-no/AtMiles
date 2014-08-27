var homepageController = function ($scope, $http) {

    $scope.queryTerm = "";

    $scope.search = function() {
        var res = $http.get("/api/Search?query=" + encodeURIComponent($scope.queryTerm));
        res.success(function(data) {
            $scope.searchResult = data;
        });
    };
    
    $scope.checkAuthenticated = function() {
        var res = $http.get("/api/test" + encodeURIComponent($scope.queryTerm));
        res.success(function (data) {
            $scope.isAuthenticated = new { good: true, name: data };
        });
        res.fail(function(error) {
                if (error.status == 401) {
                    $scope.isAuthenticated = new { good: false, name: "unknown" };
                } else {
                    alert(error);
                }
            }
        );
    }

    angular.element(document).ready(function () {
        checkAuthenticated();
    });
}