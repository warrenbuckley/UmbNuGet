angular.module("umbraco").controller("NuGet.SearchController",
    function ($scope, nugetResource) {

        //Set is loading (on init set to false)
        $scope.isLoading = false;

        //When search button is clicked
        $scope.performSearch = function (searchTerm) {

            nugetResource.searchPackages(searchTerm).then(function (response) {

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
        $scope.getPage = function (searchTerm, pageNumber) {

            //Set is loading to true again
            $scope.isLoading = true;
            
            nugetResource.searchPackagesPage(searchTerm, pageNumber).then(function (response) {

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