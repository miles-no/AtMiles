(function (module) {
     module.factory('DataFactory', function($http){
       return {
           search: function(apiRoot, searchTerm, take, skip) {
               return $http({
                   method: 'GET',
                   url: apiRoot +"/search?take=" + take + "&skip=" + skip + "&query=" + encodeURIComponent(searchTerm),
                   withCredentials: true
               });
           }
       }
    });
} (angular.module('AtMiles')));