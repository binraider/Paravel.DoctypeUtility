//(function () {
//    'use strict';

//    function doctypeUtilityController(doctypeUtilityService, editorService, contentResource, notificationsService, overlayService) {

angular.module('umbraco').controller('doctypeUtilityController', ['$scope', 'editorState', 'doctypeUtilityService',
    function ($scope, editorState, doctypeUtilityService) {


        var vm = this;
        vm.loaded = false;
        vm.dtloaded = false;
        vm.sites = [];      // array of sites
        vm.doctypes = []; // Array of doctype items
        vm.ListSites = ListSites;
        vm.ListDifferences = ListDifferences;

        init();     

        function init() {
            ListSites();
        }

        function ListSites() {
            doctypeUtilityService.ListSites()
                .then(function (data) {
                    vm.loaded = false;
                    console.log(data.data);
                    vm.sites = data.data;
                    vm.loaded = true;
                }, function (error) {
                    console.warn(error);
                    notificationsService.error('Error', 'Unable to get Doctype utility dashboard');
                });
        }

        function ListDifferences(siteName) {
            doctypeUtilityService.ListDifferences(siteName)
                .then(function (data) {
                    vm.dtloaded = false;
                    console.log(data.data);
                    vm.doctypes = data.data;
                    vm.dtloaded = true;
                }, function (error) {
                    console.warn(error);
                    notificationsService.error('Error', 'Unable to get Doctype differences for ' + siteName);
                });
        }            
    }
]);

//    angular.module('umbraco').controller('doctypeUtilityController', doctypeUtilityController);
//})();
