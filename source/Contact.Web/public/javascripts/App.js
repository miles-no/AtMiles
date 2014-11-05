var atMiles = angular.module('AtMiles', ['auth0', 'angular-storage', 'ngRoute', 'infinite-scroll']);
atMiles.config(function ($locationProvider, $httpProvider, authProvider, $routeProvider) {

    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    });
    authProvider.init({
        domain: 'atmiles.auth0.com',
        clientID: '6jsWdVCPDiKSdSKi2n7naqmy7eeO703H',
        callbackURL: location.href
    });
    $httpProvider.interceptors.push('authInterceptor');
    $routeProvider
        .when("/login", {templateUrl: "partial/login", controller: "LoginController"})
        .when("/search", {templateUrl: "partial/search", controller: "SearchController"})
        .otherwise({ redirectTo: "/search"});
});
atMiles.run(function ($rootScope, auth, store) {
    // This hooks al auth events to check everything as soon as the app starts
    auth.hookEvents();

    $rootScope.$on('$locationChangeStart', function() {
        if (!auth.isAuthenticated) {
            var token = store.get('token');
            if (token) {
                auth.authenticate(store.get('profile'), token);
            }
        }
    });
});