(function () {
    'use strict';

    function doctypeUtilityService($http) {

        var serviceRoot = "/umbraco/backoffice/api/DoctypeUtilityDashboard/";

        let resource = {
            ListSites: ListSites,
            ListDifferences: ListDifferences
        }

        function ListSites() {
            return $http.get(`${serviceRoot}ListSites`);
        }  
        function ListDifferences(key) {
            return $http.get(`${serviceRoot}ListDifferences?key=${key}`);
        }

        return resource;
    };

    angular.module('umbraco').factory('doctypeUtilityService', doctypeUtilityService);
})();