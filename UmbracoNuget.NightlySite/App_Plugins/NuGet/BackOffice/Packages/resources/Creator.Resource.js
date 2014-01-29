angular.module("umbraco.resources")
    .factory("creatorResource", function ($http) {
        return {

            getUmbracoInfo: function () {
                //Used to get info about the Umbraco install
                //Eg Macros, Templates, CSS for user to pick & choose whats in the package
                return $http.get("NuGet/CreatorApi/GetUmbracoInfo");
            },

            savePackage: function(package) {
                //Post the package to the WebAPI to save it

                //Bubble up errors - when saving check to see that PackageId does not exist locally (maybe even on MyGet?)

            },

            getPackage: function (packageId) {
                //Get the package from the WebAPI  based on packageId - be used to hydrate form to edit the package

            },

            getFiles: function(folderPath) {
                //Used to list files & folders in a folder
                //This is used directly in the File Picker Dialog
                return $http.get("NuGet/CreatorApi/GetFilesInFolder?folderPath=" + folderPath);
            },

            
        };
    });