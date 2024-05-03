
angular.module('umbraco').controller('doctypeUtilityController', ['$scope', 'editorState', 'doctypeUtilityService',
    function ($scope, editorState, doctypeUtilityService) {


        var vm = this;
        vm.loaded = false;
        vm.dtloaded = false;
        vm.sitesMissing = false;
        vm.sites = [];      // array of sites
        vm.doctypes = []; // Array of doctype items
        vm.ListSites = ListSites;
        vm.ListDifferences = ListDifferences;
        vm.ShowItems = ShowItems;
        vm.localProps = {}; // Array of doctype properties
        vm.remoteProps = {}; // Array of doctype properties
        vm.selectedDoctype = {};
        vm.selectedDoctypeLoaded = false;

        init();

        function init() {
            ListSites();
        }

        function ListSites() {
            doctypeUtilityService.ListSites()
                .then(function (data) {
                    vm.loaded = false;
                    //console.log(data.data);
                    vm.sites = data.data;
                    vm.loaded = true;
                    if (vm.sites.length == 0) {
                        vm.sitesMissing = true;
                    }
                }, function (error) {
                    console.warn(error);
                    notificationsService.error('Error', 'Unable to get Doctype utility dashboard');
                });
        }

        function ListDifferences(siteName) {
            vm.selectedDoctypeLoaded = false;
            vm.selectedDoctype = {};
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
        function ShowItems(localAlias, remoteAlias) {
            var alias = localAlias;
            if (localAlias === "") {
                alias = remoteAlias;
            }
            vm.selectedDoctypeLoaded = false;
            vm.selectedDoctype = {};
            for (i = 0; i < vm.doctypes.Doctypes.length; i++) {
                if (vm.doctypes.Doctypes[i].Local.Alias === alias || vm.doctypes.Doctypes[i].Remote.Alias === alias) {
                    vm.selectedDoctype = vm.doctypes.Doctypes[i];
                    vm.selectedDoctypeLoaded = true;
                    break;
                }
            }
        }
    }
]);

