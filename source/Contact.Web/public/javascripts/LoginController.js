(function (module) {
    module.controller('LoginController', function($scope, $rootScope){
        $scope.login = function () {

            auth.signin({
                popup: true
            }, function (profile, id_token) {
                store.set('profile', profile);
                store.set('token', id_token);
                $rootScope.$broadcast("loggedIn",id_token);
            }, function () {
                $rootScope.$broadcast("loggedOut");
            });

        }

        $scope.logout = function () {
            auth.signout();
            $rootScope.$broadcast("loggedOut");
        };

    });
}(angular.module('AtMiles')));
