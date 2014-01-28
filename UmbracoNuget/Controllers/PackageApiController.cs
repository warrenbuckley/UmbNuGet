using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NuGet;
using Umbraco.Core;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using UmbracoNuget.Helpers;
using UmbracoNuget.Models;
using UmbracoNuget.Services;
using PackageHelper = UmbracoNuget.Helpers.PackageHelper;
using umb = Umbraco.Web;

namespace UmbracoNuget.Controllers
{
    [PluginController("NuGet")]
    public class PackageApiController : UmbracoAuthorizedApiController
    {
        public const int PageSize = 9;

        public PackagesResponse GetPackages(string sortBy, int page = 1)
        {
            var zeroPageIndex = page - 1;

            var packageManager  = PackageManagerService.Instance.PackageManager;
            var repo            = packageManager.SourceRepository;

            //Get the number of packages in the repo (latest version)
            //Is there a way - to get this only once as it won't change between pages I hope?!
            var totalcount = repo.GetPackages().Where(x => x.IsLatestVersion).Count();
            var totalPages = (int)Math.Ceiling((double)totalcount / PageSize);

            //Paging from here
            //http://bitoftech.net/2013/11/25/implement-resources-pagination-asp-net-web-api/

            var prevLink = zeroPageIndex > 0 ? (page - 1).ToString() : string.Empty;
            var nextLink = zeroPageIndex < totalPages - 1 ? (page + 1).ToString() : string.Empty;

            List<IPackage> packages = new List<IPackage>();


            switch (sortBy)
            {
                case "downloads":
                    packages = repo.GetPackages()
                        .Where(x => x.IsLatestVersion)
                        .OrderByDescending(x => x.DownloadCount)
                        .Skip(zeroPageIndex * PageSize)
                        .Take(PageSize).ToList();

                    break;

                case "recent":
                    packages = repo.GetPackages()
                        .Where(x => x.IsLatestVersion)
                        .OrderByDescending(x => x.Published)
                        .Skip(zeroPageIndex * PageSize)
                        .Take(PageSize).ToList();
                    break;

                case "a-z":
                    packages = repo.GetPackages()
                        .Where(x => x.IsLatestVersion)
                        .OrderBy(x => x.Id)
                        .Skip(zeroPageIndex * PageSize)
                        .Take(PageSize).ToList();
                    break;

                default:
                    packages = repo.GetPackages()
                        .Where(x => x.IsLatestVersion)
                        .OrderByDescending(x => x.DownloadCount)
                        .Skip(zeroPageIndex * PageSize)
                        .Take(PageSize).ToList();
                    break;
            }


            //The rows we will return
            var rows = new List<Row>();

            foreach (IEnumerable<IPackage> row in packages.InGroupsOf(3))
            {
                var packagesInRow = new List<Package>();

                foreach (IPackage package in row)
                {
                    var packageToAdd                = new Package();
                    packageToAdd.Authors            = package.Authors;
                    packageToAdd.Description        = package.Description;
                    packageToAdd.DownloadCount      = package.DownloadCount.ToString("##,###,###");
                    packageToAdd.IconUrl            = package.IconUrl;
                    packageToAdd.Id                 = package.Id;
                    packageToAdd.ProjectUrl         = package.ProjectUrl;
                    packageToAdd.Published          = package.Published;
                    packageToAdd.Summary            = package.Summary;
                    packageToAdd.Tags               = package.Tags;
                    packageToAdd.Title              = package.Title;
                    packageToAdd.Version            = package.Version;

                    //Add the package to the row object
                    packagesInRow.Add(packageToAdd);
                }

                //Add the row to to the list of rows
                var packageRow      = new Row();
                packageRow.Packages = packagesInRow;

                rows.Add(packageRow);
            }
            
            //Build up object to return
            var packageResponse             = new PackagesResponse();
            packageResponse.Rows            = rows;
            packageResponse.TotalItems      = totalcount;
            packageResponse.TotalPages      = totalPages;
            packageResponse.CurrentPage     = page;
            packageResponse.PreviousLink    = prevLink;
            packageResponse.NextLink        = nextLink;

            //Return the package response
            return packageResponse;
        }

        [HttpGet]
        public PackagesResponse SearchPackages(string searchTerm, int page = 1)
        {
            var zeroPageIndex = page - 1;

            var packageManager  = PackageManagerService.Instance.PackageManager;
            var repo            = packageManager.SourceRepository;
            var searchPackages  = repo.Search(searchTerm, false);

            //Search for packages with search term
            var packages = searchPackages
                .Where(x => x.IsLatestVersion)
                .OrderByDescending(x => x.DownloadCount)
                .Skip(zeroPageIndex * PageSize)
                .Take(PageSize).ToList();

            //Get the number of packages in the repo (latest version)

            var totalcount = searchPackages.Where(x => x.IsLatestVersion).Count();
            var totalPages = (int)Math.Ceiling((double)totalcount / PageSize);

            //Paging from here
            //http://bitoftech.net/2013/11/25/implement-resources-pagination-asp-net-web-api/

            var prevLink = zeroPageIndex > 0 ? (page - 1).ToString() : string.Empty;
            var nextLink = zeroPageIndex < totalPages - 1 ? (page + 1).ToString() : string.Empty;


            //The rows we will return
            var rows = new List<Row>();

            foreach (IEnumerable<IPackage> row in packages.InGroupsOf(3))
            {
                var packagesInRow = new List<Package>();

                foreach (IPackage package in row)
                {
                    var packageToAdd                = new Package();
                    packageToAdd.Authors            = package.Authors;
                    packageToAdd.Description        = package.Description;
                    packageToAdd.DownloadCount      = package.DownloadCount.ToString("##,###,###");
                    packageToAdd.IconUrl            = package.IconUrl;
                    packageToAdd.Id                 = package.Id;
                    packageToAdd.ProjectUrl         = package.ProjectUrl;
                    packageToAdd.Published          = package.Published;
                    packageToAdd.Summary            = package.Summary;
                    packageToAdd.Tags               = package.Tags;
                    packageToAdd.Title              = package.Title;
                    packageToAdd.Version            = package.Version;

                    //Add the package to the row object
                    packagesInRow.Add(packageToAdd);
                }

                //Add the row to to the list of rows
                var packageRow      = new Row();
                packageRow.Packages = packagesInRow;

                rows.Add(packageRow);
            }

            //Build up object to return
            var packageResponse             = new PackagesResponse();
            packageResponse.Rows            = rows;
            packageResponse.TotalItems      = totalcount;
            packageResponse.TotalPages      = totalPages;
            packageResponse.CurrentPage     = page;
            packageResponse.PreviousLink    = prevLink;
            packageResponse.NextLink        = nextLink;

            //Return the package response
            return packageResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetHasInstalledPackages()
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Count number of installed packages
            var installedPackages = packageManager.LocalRepository.GetPackages().Count();

            return installedPackages > 0;

        }

        /// <summary>
        /// 
        /// <returns></returns>
        public bool GetPackageInstall(string packageID, string version)
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            bool isInstalled = false;

            //Install package
            try
            {
                var packageVersion = SemanticVersion.Parse(version);

                //Install the package...
                packageManager.InstallPackage(packageID, packageVersion, false, false);

                //Set flag to true
                isInstalled = true;

            }
            catch (Exception)
            {
                //Some error - set flag to false
                isInstalled = false;
            }

            //Returned bool if it's installed or not
            return isInstalled;        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageID"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool GetPackageUninstall(string packageID, string version)
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            bool isUninstalled = false;

            //Install package
            try
            {
                var packageVersion = SemanticVersion.Parse(version);

                //Install the package...
                packageManager.UninstallPackage(packageID, packageVersion, true, true);

                //Set flag to true
                isUninstalled = true;

            }
            catch (Exception)
            {
                //Some error - set flag to false
                isUninstalled = false;
            }

            //Returned bool if it's uninstalled or not
            return isUninstalled;      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        public PackageDetails GetPackageDetail(string packageID)
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Go & find the package by the ID
            var findPackages = packageManager.SourceRepository.FindPackagesById(packageID);

            //Latest Package
            var latestPackage = findPackages.SingleOrDefault(x => x.IsLatestVersion);

            //Can't find the package from the ID - return Null
            if (latestPackage == null)
            {
                return null;
            }

            //For each package we find add the download count so we have a total download count
            var totalDownloads = 0;

            //Loop over all versions of the package
            foreach (var package in findPackages)
            {
                totalDownloads += package.DownloadCount;
            }

            bool isInstalled = false;

            //Try & find package in local repo
            var tryFindInLocalRepo = packageManager.LocalRepository.FindPackage(packageID);

            //Found it - set flag to true
            if (tryFindInLocalRepo != null)
            {
                isInstalled = true;
            }

            bool hasAnUpdate = false;

            //Only try to check if package has an update if it's installed
            //No point checkiing otherwise
            if (isInstalled)
            {
                //Run helper to see if package has an update
                hasAnUpdate = PackageHelper.PackageHasUpdates(tryFindInLocalRepo.Id, tryFindInLocalRepo.Version.ToString());
            }

            //Build up the response we will return
            var packageDetails                          = new PackageDetails();
            packageDetails.AllDownloadsCount            = totalDownloads.ToString("##,###,###");
            packageDetails.AssemblyReferences           = latestPackage.AssemblyReferences;
            packageDetails.Authors                      = latestPackage.Authors;
            packageDetails.BuildFiles                   = latestPackage.GetBuildFiles();
            packageDetails.ContentFiles                 = latestPackage.GetContentFiles();
            packageDetails.DependencySets               = latestPackage.DependencySets;
            packageDetails.Description                  = latestPackage.Description;
            packageDetails.DownloadCount                = latestPackage.DownloadCount.ToString("##,###,###");
            packageDetails.HasAnUpdate                  = hasAnUpdate;
            packageDetails.IconUrl                      = latestPackage.IconUrl;
            packageDetails.Id                           = latestPackage.Id;
            packageDetails.IsAlreadyInstalled           = isInstalled;
            packageDetails.LibFiles                     = latestPackage.GetLibFiles();
            packageDetails.PackageAssemblyReferences    = latestPackage.PackageAssemblyReferences;
            packageDetails.ProjectUrl                   = latestPackage.ProjectUrl;
            packageDetails.Published                    = latestPackage.Published.HasValue ? latestPackage.Published.Value.ToString("dd MMMM yyyy @ HH:mm") : string.Empty;
            packageDetails.SatelliteFiles               = latestPackage.GetSatelliteFiles();
            packageDetails.Summary                      = latestPackage.Summary;
            packageDetails.Tags                         = latestPackage.Tags;
            packageDetails.Title                        = latestPackage.Title;
            packageDetails.ToolFiles                    = latestPackage.GetToolFiles();
            packageDetails.Version                      = latestPackage.Version;

            //Return the found package from the repo
            return packageDetails;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PackagesResponse GetInstalledPackages(int page = 1)
        {
            var zeroPageIndex = page - 1;

            var installedPackages = PackageHelper.ListInstalledPackages();

            //Get the number of packages in the repo (latest version)
            //Is there a way - to get this only once as it won't change between pages I hope?!
            var totalcount = installedPackages.Count();
            var totalPages = (int)Math.Ceiling((double)totalcount / PageSize);

            //Paging from here
            //http://bitoftech.net/2013/11/25/implement-resources-pagination-asp-net-web-api/

            var prevLink = zeroPageIndex > 0 ? (page - 1).ToString() : string.Empty;
            var nextLink = zeroPageIndex < totalPages - 1 ? (page + 1).ToString() : string.Empty;

            //Packages...
            List<IPackage> packages = installedPackages
                .OrderBy(x => x.Id)
                .Skip(zeroPageIndex * PageSize)
                .Take(PageSize).ToList();


            //The rows we will return
            var rows = new List<Row>();

            foreach (IEnumerable<IPackage> row in packages.InGroupsOf(3))
            {
                var packagesInRow = new List<Package>();

                foreach (IPackage package in row)
                {
                    var packageToAdd            = new Package();
                    packageToAdd.Authors        = package.Authors;
                    packageToAdd.Description    = package.Description;
                    packageToAdd.DownloadCount  = package.DownloadCount.ToString("##,###,###");
                    packageToAdd.IconUrl        = package.IconUrl;
                    packageToAdd.Id             = package.Id;
                    packageToAdd.ProjectUrl     = package.ProjectUrl;
                    packageToAdd.Published      = package.Published;
                    packageToAdd.Summary        = package.Summary;
                    packageToAdd.Tags           = package.Tags;
                    packageToAdd.Title          = package.Title;
                    packageToAdd.Version        = package.Version;

                    //Add the package to the row object
                    packagesInRow.Add(packageToAdd);
                }

                //Add the row to to the list of rows
                var packageRow = new Row();
                packageRow.Packages = packagesInRow;

                rows.Add(packageRow);
            }

            //Build up object to return
            var packageResponse             = new PackagesResponse();
            packageResponse.Rows            = rows;
            packageResponse.TotalItems      = totalcount;
            packageResponse.TotalPages      = totalPages;
            packageResponse.CurrentPage     = page;
            packageResponse.PreviousLink    = prevLink;
            packageResponse.NextLink        = nextLink;

            //Return the package response
            return packageResponse;
        }

        public PackagesResponse GetLocalPackages(int page = 1)
        {
            return null;
        }

        [HttpGet]
        public IEnumerable<Package> DialogSearchPackages(string searchTerm)
        {
            var packageManager  = PackageManagerService.Instance.PackageManager;
            var repo            = packageManager.SourceRepository;
            var searchPackages  = repo.Search(searchTerm, false);

            //Search for packages with search term
            var packages = searchPackages
                .Where(x => x.IsLatestVersion)
                .OrderByDescending(x => x.DownloadCount).ToList();

            var packageList = new List<Package>();

            foreach (IPackage package in packages)
            {
                var packageToAdd            = new Package();
                packageToAdd.Authors        = package.Authors;
                packageToAdd.Description    = package.Description;
                packageToAdd.DownloadCount  = package.DownloadCount.ToString("##,###,###");
                packageToAdd.IconUrl        = package.IconUrl;
                packageToAdd.Id             = package.Id;
                packageToAdd.ProjectUrl     = package.ProjectUrl;
                packageToAdd.Published      = package.Published;
                packageToAdd.Summary        = package.Summary;
                packageToAdd.Tags           = package.Tags;
                packageToAdd.Title          = package.Title;
                packageToAdd.Version        = package.Version;

                //Add the package to the row object
                packageList.Add(packageToAdd);
            }

            return packageList;
        }

    }
}
