angular.module("umbraco.resources")
    .factory("nugetResource", function ($http) {
        return {
            getPackages: function (sortBy) {
                return $http.get("NuGet/PackageApi/GetPackages?sortBy=" + sortBy);
            },

            getPackagePage: function (sortBy, pageNumber) {
                return $http.get("NuGet/PackageApi/GetPackages?sortBy=" + sortBy + "&page=" + pageNumber);
            },

            searchPackages: function (searchTerm, pageNumber) {
                return $http.get("NuGet/PackageApi/SearchPackages?searchTerm=" + searchTerm);
            },

            searchPackagesPage: function (searchTerm, pageNumber) {
                return $http.get("NuGet/PackageApi/SearchPackages?searchTerm=" + searchTerm + "&page=" + pageNumber);
            },

            getPackageDetail: function (packageID) {
                return $http.get("NuGet/PackageApi/GetPackageDetail?packageID=" + packageID);
            },

            installPackage: function (packageID, version) {
                return $http.get("NuGet/PackageApi/GetPackageInstall?packageID=" + packageID + "&version=" + version);
            },

            uninstallPackage: function (packageID, version) {
                return $http.get("NuGet/PackageApi/GetPackageUninstall?packageID=" + packageID + "&version=" + version);
            },

            updatePackage: function (packageID, version) {
                return $http.get("NuGet/PackageApi/GetPackageUpdate?packageID=" + packageID + "&version=" + version);
            },
            
            getInstalledPackages: function () {
                return $http.get("NuGet/PackageApi/GetInstalledPackages");
            },

            getInstalledPackagesPage: function (pageNumber) {
                return $http.get("NuGet/PackageApi/GetInstalledPackages?page=" + pageNumber);
            },

            getLocalCreatedPackages: function () {
                return $http.get("NuGet/PackageApi/GetLocalPackages");
            },

            getLocalCreatedPackagePages: function (pageNumber) {
                return $http.get("NuGet/PackageApi/GetLocalPackages?page=" + pageNumber);
            },

            //Search Packages without paging for dialog
            dialogSearchPackages: function (searchTerm) {
                return $http.get("NuGet/PackageApi/DialogSearchPackages?searchTerm=" + searchTerm);
            },

        };
    });