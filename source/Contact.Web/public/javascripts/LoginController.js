(function (module) {
    module.controller('LoginController', function($scope, $rootScope){
        //redirect to search page if authenticated
        if($rootScope.isAuthenticated) {
            $location.path('/search');
        }

        $scope.doLogin = function () {
            $rootScope.$broadcast('doLogin');
        }

        $scope.doLogout = function () {
            $rootScope.$broadcast('doLogout');
        }
    });
}(angular.module('AtMiles')));
