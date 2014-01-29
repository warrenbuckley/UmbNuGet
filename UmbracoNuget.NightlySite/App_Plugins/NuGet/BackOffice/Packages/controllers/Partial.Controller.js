angular.module("umbraco").controller("NuGet.PartialController",
    function ($scope, $routeParams) {

        /*Route Params */
        /*
        {
            section: "nuget", 
            tree: "Packages", 
            method: "edit", 
            id: "settings"
        } 
        */

        //Currently loading /umbraco/general.html
        //Need it to look at /App_Plugins/
        var viewName    = $routeParams.id;
        viewName        = viewName.replace('%20', '-').replace(' ', '-');

        // Partial Type (edit, view, list)
        var partialType = $routeParams.method;
        
        // /App_Plugins/NuGet/backoffice/Packages/partials/edit/settings
        // /App_Plugins/NuGet/backoffice/Packages/partials/view/browse
        // /App_Plugins/NuGet/backoffice/Packages/partials/list/local-packages
        $scope.templatePartialURL = '../App_Plugins/NuGet/backoffice/Packages/partials/' + partialType + '/' + viewName + '.html';
    });