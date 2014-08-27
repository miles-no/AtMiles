var homepageController = function ($scope, $http) {

    $scope.apiRoot = "http://localhost:56548";

    $scope.queryTerm = "";

   
    $scope.queryTerm = "";

    $scope.search = function () {
        var res = $http.get($scope.apiRoot + "/api/Search?query=" + encodeURIComponent($scope.queryTerm));
        res.success(function (data) {
            $scope.searchResult = data;
        });
    };

    $scope.checkAuthenticated = function() {
        var res = $http({
            method: 'GET',
            url: $scope.apiRoot + "/api/test",
            withCredentials: true
        });
        res.success(function(data) {
            $scope.isAuthenticated =  { good: true, name: data.replace(/"/g, "") };
        });
        res.error(function(errorData,status) {
                if (status == 401) {
                    $scope.isAuthenticated = { good: false, name: "unknown" };
                } else {
                    alert(errorData);
                }
            }
        );
    }

    angular.element(document).ready(function() {
        $scope.checkAuthenticated();
    });

}