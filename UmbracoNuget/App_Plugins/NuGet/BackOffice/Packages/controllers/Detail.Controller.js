angular.module("umbraco").controller("NuGet.DetailController",
    function ($scope, $routeParams, nugetResource, notificationsService, navigationService, editorState) {

        //Set isLoading to true on init
        $scope.isLoading = true;

        //Set isInstalling to false on init
        $scope.isInstalling = false;

        //Set isUninstalling to false on init
        $scope.isUninstalling = false;
       

        //Get the PackageID - in this case the ID of the URL/Route
        //http://localhost:64700/umbraco/#/NuGet/Packages/detail/jQuery
        //eg: jQuery
        var packageID = $routeParams.id;

        //Go & Get Package Details from WebAPI via Resource
        nugetResource.getPackageDetail(packageID).then(function (response) {

            //Now we have JSON data let's turn off the loading message/spinner
            $scope.isLoading = false;

            //Set a scope object from the JSON we get back
            $scope.package = response.data;
        });

        //Install Button Clicked
        $scope.installPackage = function (packageID, version) {

            $scope.isInstalling = true;

            nugetResource.installPackage(packageID, version).then(function (response) {

                //Package is installed
                $scope.isInstalling = false;

                //Get response from api (returns true or false)
                var wasPackagedInstalled = response.data;

                //Show success or error notification message
                if (wasPackagedInstalled) {
                    notificationsService.success("Installed Package from Repo", packageID);

                    //Set Package Already installed to true
                    $scope.package.IsAlreadyInstalled   = true;
                    $scope.package.HasAnUpdate          = false;

                }
                else {
                    notificationsService.error("Problem Installing Package from Repo", packageID);
                }
                

                //Reload tree/node
                navigationService.syncTree({ tree: "nuget", path: "-1", forceReload: true }).then(function (syncArgs) {

                    console.log(syncArgs);
                    navigationService.reloadNode(syncArgs.node);

                });

               
            });
        };

        //Update Package Button Clicked
        $scope.updatePackage = function (packageID, version) {

            $scope.isInstalling = true;

            nugetResource.updatePackage(packageID, version).then(function (response) {

                //Package is installed/updated
                $scope.isInstalling = false;

                //Get response from api (returns true or false)
                var wasPackagedInstalled = response.data;

                //Show success or error notification message
                if (wasPackagedInstalled) {
                    notificationsService.success("Installed Package Update from Repo", packageID);

                    //Set Package Already installed to true
                    $scope.package.IsAlreadyInstalled   = true;
                    $scope.package.HasAnUpdate          = false;

                }
                else {
                    notificationsService.error("Problem Installing Package Update from Repo", packageID);
                }

                //Reload tree/node
                navigationService.syncTree({ tree: "nuget", path: "-1", forceReload: true }).then(function (syncArgs) {

                    console.log(syncArgs);
                    navigationService.reloadNode(syncArgs.node);

                });

            });
        };

        //Uninstall Package Button Clicked
        $scope.uninstallPackage = function (packageID, version) {

            $scope.isUninstalling = true;

            nugetResource.uninstallPackage(packageID, version).then(function (response) {

                //Package is installed/updated
                $scope.isUninstalling = false;

                //Get response from api (returns true or false)
                var wasPackagedUninstalled = response.data;

                //Show success or error notification message
                if (wasPackagedUninstalled) {
                    notificationsService.success("Uninstalled Package from Repo", packageID);

                    //Set Package Already installed to false (as now removed)
                    $scope.package.IsAlreadyInstalled   = false;
                    $scope.package.HasAnUpdate          = false;
                }
                else {
                    notificationsService.error("Problem Uninstalling Package from Repo", packageID);
                }

                //Reload tree/node
                navigationService.syncTree({ tree: 'nuget', path: ["-1"], forceReload: true });

            });
        };

    });