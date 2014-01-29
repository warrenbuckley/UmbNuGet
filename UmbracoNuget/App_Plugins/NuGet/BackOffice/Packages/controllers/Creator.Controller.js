angular.module("umbraco").controller("NuGet.CreatorController",
    function ($scope, creatorResource, dialogService) {

        //On init of view - go & fetch info about the umbraco install
        creatorResource.getUmbracoInfo().then(function (response) {

            $scope.dataTypes        = response.data.DataTypes;
            $scope.macros           = response.data.Macros;
            $scope.docTypes         = response.data.ContentTypes;
            $scope.templates        = response.data.Templates;
            $scope.stylesheets      = response.data.Stylesheets;
            $scope.languages        = response.data.Languages;
            $scope.dictionaryItems  = response.data.DictionaryItems;

        });

        //Folder objects we pass through to our dialog picker
        $scope.binFolder    = { FolderName: 'bin', RelPath: '/bin' };
        $scope.rootFolder   = { FolderName: '/', RelPath: '/' };


        //Click button - Pick Content Node
        $scope.pickContentNode = function () {

            //When button clicked - Let's open the content picker dialog on the right
            dialogService.contentPicker({ callback: itemPicked });
            
            //When the node has been picked - do this...
            function itemPicked(pickedItem) {

                //Log the JSON object we get back from the dialog picker
                console.log(pickedItem);

                //Set the picked item to our scope
                $scope.nodePicked = pickedItem;
            }
        };

        //Click Button - Save Package
        $scope.savePackage = function (packageData) {

            alert('Saving Package');

        };

        //Click Button - Pick Files
        $scope.pickFiles = function (folder) {

            //Open a dialog - passing in a ton of options
            dialogService.open({ template: '../App_Plugins/NuGet/BackOffice/Packages/partials/dialogs/file-picker.html', callback: filesPicked, dialogData: { Folder: folder } });


            //When the node has been picked - do this...
            function filesPicked(pickedFiles) {

                //Log the JSON object we get back from the dialog picker
                console.log(pickedFiles);
                
            }
        };

        //Click Button - Pick Dependencies dialog
        $scope.pickDependencies = function() {

            //Open a dialog - passing in a ton of options
            dialogService.open({ template: '../App_Plugins/NuGet/BackOffice/Packages/partials/dialogs/dependency-picker.html', callback: packagesPicked });


            //When the node has been picked - do this...
            function packagesPicked(pickedPackages) {

                //Log the JSON object we get back from the dialog picker
                console.log(pickedPackages);

            }

        };

    });