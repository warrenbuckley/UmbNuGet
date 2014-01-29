angular.module("umbraco").controller("NuGet.FilePickerController",
    function ($scope, $element, creatorResource) {

        //Get the folder path passed into the dialog as dialogData
        var startFolder = $scope.dialogData.Folder;

        //Log some info
        console.log('Get files & folders in: ' + startFolder.RelPath);

        //Folder Paths object for breadcrumb style navigation
        $scope.folderPaths = [startFolder];


        //Go & get the files & folders from the folder path via WebAPI
        getFolder(startFolder.RelPath);


        //CLICK - Folder
        $scope.selectFolder = function (folder) {

            console.log('Selected Folder');
            console.log(folder);

            //Add the new chosen folder to the folderPaths object
            $scope.folderPaths.push(folder);
            
            //Log some info
            console.log('Selected Folder - Get files & folders in: ' + folder.RelPath);

            //Go & get the new folder we clicked on...
            getFolder(folder.RelPath);

        };

        //CLICK - BreadCrumb Folder
        $scope.goToFolder = function(folder) {

            console.log('Goto Folder');
            console.log(folder);

            //Go & get the new folder we clicked on...
            getFolder(folder.RelPath);

            //Stop at folder (either stop to root or bin folder)
            var stopAtFolder = $scope.folderPaths[0];

            
            
            //Remove items in folderPaths obj array that are no longer needed
            var positionOfItem =
                $scope.folderPaths.map(function(e) {
                    return e.RelPath;
                }).indexOf(folder.RelPath);

            console.log('Position of item in array: ' + positionOfItem);

            //Get our new partial array
            var newFolderPaths = $scope.folderPaths.slice(0, positionOfItem + 1);

            console.log('New Array');
            console.log(newFolderPaths);

            //Reset our folder array
            $scope.folderPaths = newFolderPaths;
            
        };


        //CLICK File - Add to selected collection
        $scope.selectFile = function (file, $event) {

            console.log($event);



            //Get the clicked dom element
            var clickedNode         = angular.element($event.target);
            var clickedNodeParent   = angular.element($event.target.parentElement);


            //Toggle CSS class 'current' to the clicked li (parent node)
            clickedNodeParent.toggleClass("current");

            //Toggle CSS class 'selected' on the div item inside the li
            clickedNode.toggleClass("selected");


            //Select the file for the picker
            $scope.select(file);

        };

        //Generic Function to run fo fetching files...
        function getFolder(folderPath) {

            //Use Creator Resource to get Files
            creatorResource.getFiles(folderPath).then(function (response) {
                $scope.folders          = response.data.Folders;
                $scope.files            = response.data.Files;
                $scope.currentFolder    = response.data.CurrentFolder;
            });

        }

        //Function to help determine if 
        $scope.isItemSelected = function (file) {

            //Check if the nuget id is in the list of selected items
            var fileRelPath = file.RelPath;
            var parentScope = $scope.$parent;
            var dialogData  = parentScope.dialogData;

            for (var i = 0; i < dialogData.selection.length ; i++) {

                console.log('Item ' + i);
                console.log(dialogData.selection[i]);
                console.log(fileRelPath);

                //If the current item in the selection keys matches the nugetId of the item in the list then return true
                if (dialogData.selection[i].RelPath === fileRelPath) {

                    console.log('isItemSelected is true');

                    return true;
                }
            }

            return false;

        };

    });