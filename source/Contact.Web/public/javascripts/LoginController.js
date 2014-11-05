(function (module) {
    module.controller('LoginController', function($scope, $rootScope){

        $scope.doLogin = function () {
            $rootScope.$broadcast('doLogin');
        }

        $scope.doLogout = function () {
            $rootScope.$broadcast('doLogout');
        }
    });
}(angular.module('AtMiles')));
