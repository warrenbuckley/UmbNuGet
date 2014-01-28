using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using NuGet;

namespace UmbracoNuget.Services
{
    
    interface IPackageManagerService
    {

    }

    public class PackageManagerService : IPackageManagerService
    {
        public const string NuGetRepoUrl = "https://packages.nuget.org/api/v2";
        public const string MyGetRepoUrl = "https://www.myget.org/F/umbraco-community/";

        #region Singleton

        protected static volatile PackageManagerService _Instance = new PackageManagerService();
        protected static object SyncRoot = new Object();

        protected PackageManagerService()
        {
            var repo        = PackageRepositoryFactory.Default.CreateRepository(MyGetRepoUrl);
            var path        = new DefaultPackagePathResolver(MyGetRepoUrl);
            var fileSystem  = new PhysicalFileSystem(HostingEnvironment.MapPath("~/App_Plugins/Packages"));
            var localRepo   = PackageRepositoryFactory.Default.CreateRepository(HostingEnvironment.MapPath("~/App_Plugins/Packages"));

            //Create a NuGet Package Manager
            PackageManager = new PackageManager(repo, path, fileSystem, localRepo);

        }


        public PackageManager PackageManager { get; set; }

        public static PackageManagerService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new PackageManagerService();
                    }
                }

                return _Instance;
            }
        }

        #endregion
    }
}


