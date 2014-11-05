(function (module) {
    module.controller('MainController', function ($scope, $http, $timeout, $location, auth, store, $rootScope) {

        $rootScope.isAuthenticated = false;

        //pick up login from Auth0
        $scope.$on('auth0.authenticated', function (prof) {

            $rootScope.isAuthenticated = true;
        });

        //listen to event from LoginController when user click login on page
        $rootScope.$on("doLogin", function() {
            $scope.login();
        });
        $rootScope.$on("doLogout", function() {
            $scope.logout();
        });

        $scope.login = function () {

            auth.signin({
                popup: true
            }, function (profile, id_token) {
                store.set('profile', profile);
                store.set('token', id_token);
                $rootScope.isAuthenticated = true;
                $location.path('/search');
            }, function () {
                $rootScope.isAuthenticated = false;
                $location.path('/login');
            });
        }

        $scope.logout = function () {
            auth.signout();
            $rootScope.isAuthenticated = false;
            $location.path('/login');
        };
    });
}(angular.module('AtMiles')));