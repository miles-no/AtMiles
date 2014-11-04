(function (module) {
    module.controller('MainController', function ($scope, $http, $timeout, $location, auth, store, $rootScope) {
        $scope.apiRoot = "https://api-at.miles.no";

        $rootScope.isAuthenticated = false;

        $scope.$on('auth0.authenticated', function (prof) {

            $rootScope.isAuthenticated = true;
        });

        $scope.$on("loggedIn", function(event, id_token) {
            $rootScope.isAuthenticated = true;
        });

        $scope.$on("loggedOut", function(event, id_token) {
            $rootScope.isAuthenticated = false;
        });

        var getCompany = function () {
            // Used to divide into companies. Useless just now
            return '';
            //var p = $location.path().split("/");
            //return p[1] || "Unknown";
        }
        var lastQueryTerm = null;
        var maxSearchResults = 21;

        var skip = 0;
        var take = maxSearchResults;

        $scope.selectedEmployee = null;
        $scope.dataLoading = false; //used to detect when we're waiting for data

        $scope.searchPerformed = false;
        $scope.moreSearchResults = false;
        $scope.searchResult = { Results: [], Skipped: 0, Total: 0 };
    });
}(angular.module('AtMiles')));