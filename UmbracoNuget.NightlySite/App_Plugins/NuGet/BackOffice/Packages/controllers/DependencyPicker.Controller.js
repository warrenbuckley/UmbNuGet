angular.module("umbraco").controller("NuGet.DependencyPickerController",
    function ($scope, $element, nugetResource) {

        //On keyup in search box - do this..
        $scope.performSearch = function() {

            //Check we have some value in the search box
            if ($scope.searchText)
            {
                //Check that the old/previous term is not the same as current search term
                if ($scope.oldSearchText !== $scope.searchText)
                {
                    //Empty out the existing results array
                    //Set the old term to the current search term so we can track next time round
                    $scope.oldSearchText    = $scope.searchText;
                    $scope.fetchingResults  = true;

                    //Go and get search results & when got a response populate searchResults object
                    nugetResource.dialogSearchPackages($scope.searchText).then(function (response) {

                        $scope.searchResults    = response.data;
                        $scope.fetchingResults  = false;
                    });

                }
            }
            else {
                $scope.searchResults = [];
                $scope.oldSearchText = "";
            }
        };

        //CLICK File - Add to selected collection
        $scope.selectPackage = function (nuget, $event) {

            console.log($event);

            //Get the clicked dom element
            var clickedNode         = angular.element($event.target);
            var clickedNodeParent   = angular.element($event.target.parentElement);

            //Toggle CSS class 'current' to the clicked li (parent node)
            clickedNodeParent.toggleClass("current");

            //Toggle CSS class 'selected' on the div item inside the li
            clickedNode.toggleClass("selected");

            //Select the package ID for the picker (not the entire object)
            $scope.select(nuget.Id);

        };

        //Function to help determine if 
        $scope.isItemSelected = function(nuget) {

            //Check if the nuget id is in the list of selected items
            var nugetId     = nuget.Id;
            var parentScope = $scope.$parent;
            var dialogData  = parentScope.dialogData;

            for (var i = 0; i < dialogData.selection.length ; i++) {

                console.log('Item ' + i);
                console.log(dialogData.selection[i]);
                console.log(nugetId);

                //If the current item in the selection keys matches the nugetId of the item in the list then return true
                if (dialogData.selection[i] === nugetId) {

                    console.log('isItemSelected is true');

                    return true;
                }
            }

            return false;

        };

    });