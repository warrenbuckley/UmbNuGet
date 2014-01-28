using System;
using NuGet;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using UmbracoNuget.Helpers;
using UmbracoNuget.Services;

namespace UmbracoNuget
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Check to see if section needs to be added
            Install.AddSection(applicationContext);

            //Check to see if language keys for section needs to be added
            Install.AddSectionLanguageKeys();

            //Add OLD Style Package Event
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;

            //Package Manager events
            var packageManger                   = PackageManagerService.Instance.PackageManager;
            packageManger.PackageInstalling     += packageManger_PackageInstalling;
            packageManger.PackageInstalled      += packageManger_PackageInstalled;
            packageManger.PackageUninstalling   += packageManger_PackageUninstalling;
            packageManger.PackageUninstalled    += packageManger_PackageUninstalled;

            /*
            //Classes/objects to look at for building/creating a package
            PackageBuilder builder = new PackageBuilder();
            OptimizedZipPackage zip = new OptimizedZipPackage();
             * 
             * ZipPackage
             * ZipPackageFile
             * OptimizedZipPackage 
             * LocalPackage
             */

        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void packageManger_PackageInstalling(object sender, NuGet.PackageOperationEventArgs e)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void packageManger_PackageInstalled(object sender, NuGet.PackageOperationEventArgs e)
        {
            //Copy the package files out to the correct locations
            e.Package.CopyPackageFiles();

            //If package has old format package.xml - lets try & improt doctypes & content etc...
            e.Package.InstallFromPackageXml();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void packageManger_PackageUninstalling(object sender, NuGet.PackageOperationEventArgs e)
        {
            //If package has old format package.xml - lets try & uninstall - Run uninstall/undo package actions
            e.Package.UninstallFromPackageXml();

            //Try & run OLD Legacy event - BeforeDelete of Package aka uninstall/remove/delete
            //umbraco.cms.businesslogic.packager.InstalledPackage.FireBeforeDelete
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void packageManger_PackageUninstalled(object sender, NuGet.PackageOperationEventArgs e)
        {
            //Try & run OLD Legacy event - AfterDelete of package aka uninstall/remove/delete
            //umbraco.cms.businesslogic.packager.InstalledPackage.FireAfterDelete

            //Remove the package files
            e.Package.RemovePackageFiles();
        }


        /// <summary>
        /// Before Delete of this NuGet Extension package
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InstalledPackage_BeforeDelete(InstalledPackage sender, System.EventArgs e)
        {
            //Check which package is being uninstalled
            if (sender.Data.Name == "NuGet")
            {
                //Start Uninstall - clean up process...
                //Uninstall.RemoveSection();
                //Uninstall.RemoveSectionLanguageKeys();
            }
        }
    }
}
