var homepageController = function ($scope, $http) {

    $scope.queryTerm = "";

    $scope.search = function() {
        var res = $http.get("/api/Search?query=" + encodeURIComponent($scope.queryTerm));
        res.success(function(data) {
            $scope.searchResult = data;
        });
    };
    

}