angular.module("umbraco").controller("NuGet.BrowseController",
    function ($scope, nugetResource) {

        //Set is loading flag until JSON comes back from oData call
        $scope.isLoading = true;

        $scope.sortBy   = "downloads";
        var initSort    = $scope.sortBy;

        nugetResource.getPackages(initSort).then(function (response) {

            //Now we have JSON data let's turn off the loading message/spinner
            $scope.isLoading = false;

            //Set a scope object from the JSON we get back
            $scope.rows             = response.data.Rows;
            $scope.packageCount     = response.data.NoResults;
            $scope.prevPageNumber   = response.data.PreviousLink;
            $scope.nextPageNumber   = response.data.NextLink;

        });

        //When sort order is updated
        $scope.updateSortOrder = function (sortBy) {

            nugetResource.getPackages(sortBy).then(function (response) {

                //Now we have JSON data let's turn off the loading message/spinner
                $scope.isLoading = false;

                //Set a scope object from the JSON we get back
                $scope.rows             = response.data.Rows;
                $scope.packageCount     = response.data.NoResults;
                $scope.prevPageNumber   = response.data.PreviousLink;
                $scope.nextPageNumber   = response.data.NextLink;

            });
        };


        
        //Get More Packages - click
        $scope.getPage = function (sortBy, pageNumber) {

            //Set is loading to true again
            $scope.isLoading = true;
            
            nugetResource.getPackagePage(sortBy, pageNumber).then(function (response) {

                //Now we have JSON data let's turn off the loading message/spinner
                $scope.isLoading = false;

                //Set a scope object from the JSON we get back
                $scope.rows             = response.data.Rows;
                $scope.packageCount     = response.data.NoResults;
                $scope.prevPageNumber   = response.data.PreviousLink;
                $scope.nextPageNumber   = response.data.NextLink;

            });
        };

    });