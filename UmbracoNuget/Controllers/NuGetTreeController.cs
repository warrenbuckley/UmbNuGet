using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using Umbraco.Core;
using umbraco.presentation.dialogs;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using UmbracoNuget.Helpers;
using UmbracoNuget.Models;

namespace UmbracoNuget.Controllers
{
    [Tree("nuget", "Packages", "NuGet Packages")]
    [PluginController("NuGet")]
    public class NuGetTreeController : TreeController
    {
        public const string mainRoute = "/nuget/Packages";

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            //check if we're rendering the local tree
            if (id == "local")
            {
                var menu = new MenuItemCollection();

                //Default Route = /App_Plugins/NuGet/backoffice/Packages/create-package.html
                //Overwriten Route = /App_Plugins/NuGet/BackOffice/Packages/partials/dialogs/create-package.html
                var createPackageMenu   = new MenuItem("create-package", "Create Package");
                createPackageMenu.Icon  = "brick"; //CSS class name without the icon- prefix - icon-brick
                createPackageMenu.NavigateToRoute("nuget/Packages/create/package");
                menu.Items.Add(createPackageMenu);

                //Default Route = /App_Plugins/NuGet/backoffice/Packages/convert-package.html
                //Overwritten Route = 
                var convertPackageMenu  = new MenuItem("convert-package", "Convert Package");
                convertPackageMenu.Icon = "cloud-upload"; //CSS class name without the icon- prefix - icon-cloud-upload
                convertPackageMenu.LaunchDialogView("/App_Plugins/NuGet/BackOffice/Packages/partials/dialogs/convert-package.html", "Convert Package");
                menu.Items.Add(convertPackageMenu);
                
                return menu;
            }

            //if not the local-packages node (aka Local Packages node) do nothing
            return null;

        }

        /// <summary>
        /// Main Overide for items in our tree for our NuGet section/app
        /// </summary>
        /// <param name="id"></param>
        /// <param name="queryStrings"></param>
        /// <returns></returns>
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            //Nodes that we will return
            var nodes = new TreeNodeCollection();

            //Add nodes
            var treeNodes = new List<SectionTreeNode>();

            //check if we're rendering the root node's children
            if (id == Constants.System.Root.ToInvariantString())
            {
                //Call GetMainTreeItems()
                treeNodes = GetMainTreeItems();
            }


            //Check if we are rendering the installed tree
            if (id == "installed")
            {
                treeNodes = GetInstalledPackagesTreeItems();
            }

            //Check if we are rendering the local tree
            if (id == "local")
            {
                //List out the locally created packages
                //App_Plugins/Created-Packages/


                //treeNodes = GetInstalledPackagesTreeItems();
            }


            //Create tree nodes
            if (treeNodes.Any())
            {
                //Write out the tree nodes...
                nodes = CreateTreeNodes(treeNodes, queryStrings);

                //Return the nodes
                return nodes;
            }


            //this tree doesn't suport rendering more than 1 level
            throw new NotSupportedException();
        }


        /// <summary>
        /// Main Tree when section loads
        /// </summary>
        /// <returns></returns>
        public List<SectionTreeNode> GetMainTreeItems()
        {
            //Add nodes
            var treeNodes = new List<SectionTreeNode>();

            //Brow Packages
            treeNodes.Add(new SectionTreeNode() { Id = "browse", Title = "Browse Packages", Icon = "icon-box-alt", Route = string.Format("{0}/view/{1}", mainRoute, "browse"), HasChildren = false });

            //Search for Packages
            treeNodes.Add(new SectionTreeNode() { Id = "search", Title = "Search for Packages", Icon = "icon-search", Route = string.Format("{0}/view/{1}", mainRoute, "search"), HasChildren = false });

            //Check we have any installed packages
            if (PackageHelper.HasInstalledPackages())
            {
                //Add installed packages item to tree
                treeNodes.Add(new SectionTreeNode() { Id = "installed", Title = "Installed Packages", Icon = "icon-box", Route = string.Format("{0}/list/{1}", mainRoute, "installed"), HasChildren = true });
            }

            //Local Packages
            //TODO: Need to check if any local created packages exist or not (HasChildren boolean)
            treeNodes.Add(new SectionTreeNode() { Id = "local", Title = "Local Packages", Icon = "icon-brick", Route = string.Format("{0}/list/{1}", mainRoute, "local-packages"), HasChildren = false });

            //Settings (Used mianly for MyGet Repo API key for pushing/publishing)
            treeNodes.Add(new SectionTreeNode() { Id = "settings", Title = "Settings", Icon = "icon-settings", Route = string.Format("{0}/edit/{1}", mainRoute, "settings"), HasChildren = false });

            //Return the list of nodes
            return treeNodes;
        }


        /// <summary>
        /// Used when Installed packages node is expanded (Fetches installed packages)
        /// </summary>
        /// <returns></returns>
        public List<SectionTreeNode> GetInstalledPackagesTreeItems()
        {
            //Add nodes
            var treeNodes = new List<SectionTreeNode>();

            //Get installed packages
            var installedPackages = PackageHelper.ListInstalledPackages();

            foreach (var package in installedPackages)
            {
                //NuGet/Packages/detail/jQuery
                treeNodes.Add(new SectionTreeNode() { Id = package.Id, Title = package.Title, Icon = "icon-box-open", Route = string.Format("{0}/detail/{1}", mainRoute, package.Id), HasChildren = false });
            }

            return treeNodes;
        }

        /// <summary>
        /// Creates the tree nodes from our list we pass in
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <param name="queryStrings"></param>
        /// <returns></returns>
        public TreeNodeCollection CreateTreeNodes(List<SectionTreeNode> treeNodes, FormDataCollection queryStrings)
        {
            //Nodes that we will return
            var nodes = new TreeNodeCollection();

            foreach (var item in treeNodes)
            {
                //When clicked - /App_Plugins/NuGet/backoffice/Packages/edit.html
                //URL in address bar - /developer/NuGet/General/someID
                var nodeToAdd = CreateTreeNode(item.Id, null, queryStrings, item.Title, item.Icon, item.HasChildren, item.Route);

                //Add it to the collection
                nodes.Add(nodeToAdd);
            }

            return nodes;
        }
    }

}
