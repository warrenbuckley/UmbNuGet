using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using NuGet;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.macro;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using UmbracoNuget.Services;


namespace UmbracoNuget.Helpers
{
    public static class PackageHelper
    {
        public static bool HasInstalledPackages()
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Count number of installed packages
            var installedPackages = packageManager.LocalRepository.GetPackages().Count();

            return installedPackages > 0;
        }

        public static List<IPackage> ListInstalledPackages()
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Return the list of packages
            return packageManager.LocalRepository.GetPackages().ToList();
        }

        public static bool HasUpdates()
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Get the current installed packages
            var installedPackages = packageManager.LocalRepository.GetPackages();

            //Get Updates for the set of installed packages
            var packageUpdates = packageManager.SourceRepository.GetUpdates(installedPackages, false, false);

            //Return bool if we have any updates
            return packageUpdates.Any();
        }

        public static List<IPackage> ListPackageUpdates()
        {
            //Get Package Manager
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Get the current installed packages
            var installedPackages = packageManager.LocalRepository.GetPackages();

            //Get Updates for the set of installed packages
            var packageUpdates = packageManager.SourceRepository.GetUpdates(installedPackages, false, false);

            return packageUpdates.ToList();
        }

        public static bool PackageHasUpdates(string packageID, string version)
        {
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Convert version to SemanticVersion
            var semVersion = SemanticVersion.Parse(version);

            //Get the package
            var getPackage = packageManager.LocalRepository.FindPackagesById(packageID).Where(x => x.Version == semVersion);

            //Get Updates for the set of installed packages
            var packageUpdates = packageManager.SourceRepository.GetUpdates(getPackage, false, false);

            //Return bool if we have any updates
            return packageUpdates.Any();
        }

        public static List<IPackage> ListUpdatesForPackage(string packageID, string version)
        {
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Convert version to SemanticVersion
            var semVersion = SemanticVersion.Parse(version);

            //Get the package
            var getPackage = packageManager.LocalRepository.FindPackagesById(packageID).Where(x => x.Version == semVersion);

            //Get Updates for the set of installed packages
            var packageUpdates = packageManager.SourceRepository.GetUpdates(getPackage, false, false);
            
            return packageUpdates.ToList();
        }

        public static string GetTotalDownloads(this IPackage packge)
        {
            var packageManager = PackageManagerService.Instance.PackageManager;

            //Go & find the package by the ID
            var findPackages = packageManager.SourceRepository.FindPackagesById(packge.Id);

            if (findPackages == null)
            {
                return string.Empty;
            }

            //For each package we find add the download count so we have a total download count
            var totalDownloads = 0;

            //Loop over all versions of the package
            foreach (var package in findPackages)
            {
                totalDownloads += package.DownloadCount;
            }

            return totalDownloads.ToString("##,###,###");
        }


        public static void CopyPackageFiles(this IPackage package)
        {
            //For Content Files Copy the files to the correct places
            foreach (var file in package.GetContentFiles())
            {
                //Copy File from package to correct location on disk
                var fileLocation = file.EffectivePath;

                //Map Path from location
                var mappedFileLocation = HostingEnvironment.MapPath("~/" + fileLocation);

                //Ensure directory exists
                if (!Directory.Exists(Path.GetDirectoryName(mappedFileLocation)))
                {
                    //Directory does NOT exist
                    //Create it
                    Directory.CreateDirectory(Path.GetDirectoryName(mappedFileLocation));
                }
                
                //Get File Contents
                var fileContents = file.GetStream();

                //Save file to disk
                //http://stackoverflow.com/questions/411592/how-do-i-save-a-stream-to-a-file
                using (var fileStream = File.Create(mappedFileLocation))
                {
                    fileContents.CopyTo(fileStream);
                }

            }

            //For Lib files (aka /bin)
            foreach (var file in package.GetLibFiles())
            {
                //Copy File from package to /bin folder
                var fileLocation = file.EffectivePath;

                //Map Path from location
                var mappedFileLocation = HostingEnvironment.MapPath("~/bin/" + fileLocation);

                //Ensure directory exists (I hope so as it's the /bin folder)
                if (!Directory.Exists(Path.GetDirectoryName(mappedFileLocation)))
                {
                    //Directory does NOT exist
                    //Create it
                    Directory.CreateDirectory(Path.GetDirectoryName(mappedFileLocation));
                }

                //Get File Contents
                var fileContents = file.GetStream();

                //Save file to disk
                //http://stackoverflow.com/questions/411592/how-do-i-save-a-stream-to-a-file
                using (var fileStream = File.Create(mappedFileLocation))
                {
                    fileContents.CopyTo(fileStream);
                }


            }
        }

        public static void RemovePackageFiles(this IPackage package)
        {
            //For Content Files remove the files off disk
            foreach (var file in package.GetContentFiles())
            {
                //Remove File from disk

                //Remove File that its package from disk
                var fileLocation = file.EffectivePath;

                //Map Path from location
                var mappedFileLocation = HostingEnvironment.MapPath("~/" + fileLocation);

                //Ensure file exists on disk
                if (File.Exists(mappedFileLocation))
                {
                    //It exists - so let's delete it
                    File.Delete(mappedFileLocation);
                }
            }

            //Remove the directories
            foreach (var dir in package.GetContentFiles())
            {
                //Remove File that its package from disk
                var fileLocation = dir.EffectivePath;

                //Map Path from location
                var mappedFileLocation = HostingEnvironment.MapPath("~/" + fileLocation);

                //Get the directory to delete
                var directoryPath = Path.GetDirectoryName(mappedFileLocation);

                //Check direcotry exists
                if (Directory.Exists(directoryPath))
                {
                    //Delete the directory
                    Directory.Delete(directoryPath);
                }

            }

            //For Lib files (aka /bin)
            foreach (var file in package.GetLibFiles())
            {
                //Remove File from to /bin folder

                //Copy File from package to /bin folder
                var fileLocation = file.EffectivePath;

                //Map Path from location
                var mappedFileLocation = HostingEnvironment.MapPath("~/bin/" + fileLocation);

                //Remove DLL from bin
                File.Delete(mappedFileLocation);
            }
        }


        public static void InstallFromPackageXml(this IPackage package)
        {
            //Check to swee if we have a package.xml file in the root of the Contents folder
            var checkForPackageXML = package.GetContentFiles().SingleOrDefault(x => x.EffectivePath.EndsWith("\\package.xml"));

            //Check to see if we found the file
            if (checkForPackageXML != null)
            {
                //Found the package.xml file - let's use it

                //Actual path to xml file
                var mappedFileLocation = HostingEnvironment.MapPath("~/" + checkForPackageXML.EffectivePath);

                //Load the package.xml file into XDocument so we can use it with packaging service
                var packageXML = new XmlDocument();
                packageXML.Load(mappedFileLocation);

                //Umbraco admin user
                var currentUser = User.GetCurrent();

                //Code Below heavily lifted from umbraco.core
                //https://github.com/umbraco/Umbraco-CMS/blob/d5d4dc95619dd510a4fa9713c593d2a918bcb43b/src/umbraco.cms/businesslogic/Packager/Installer.cs

                //XElement - DataTypeDefinitions
                //Xml as XElement which is used with the new PackagingService
                var rootElement         = packageXML.DocumentElement.GetXElement();
                var packagingService    = ApplicationContext.Current.Services.PackagingService;


                //Data Types
                var dataTypeElement = rootElement.Descendants("DataTypes").FirstOrDefault();
                if (dataTypeElement != null)
                {
                    var dataTypeDefinitions = packagingService.ImportDataTypeDefinitions(dataTypeElement);
                }

                //Languages
                var languageItemsElement = rootElement.Descendants("Languages").FirstOrDefault();
                if (languageItemsElement != null)
                {
                    var insertedLanguages = packagingService.ImportLanguages(languageItemsElement);
                }

                //Dictionary Items
                var dictionaryItemsElement = rootElement.Descendants("DictionaryItems").FirstOrDefault();
                if (dictionaryItemsElement != null)
                {
                    var insertedDictionaryItems = packagingService.ImportDictionaryItems(dictionaryItemsElement);
                }


                //Macros
                foreach (XmlNode n in packageXML.DocumentElement.SelectNodes("//macro"))
                {
                    //WB: Hopefully this gets a nice packagingService too...
                    Macro m = Macro.Import(n);
                }

                //Templates
                var templateElement = rootElement.Descendants("Templates").FirstOrDefault();
                if (templateElement != null)
                {
                    var templates = packagingService.ImportTemplates(templateElement);
                }


                //DocumentTypes
                //Check whether the root element is a doc type rather then a complete package
                var docTypeElement = rootElement.Name.LocalName.Equals("DocumentType") ||
                                     rootElement.Name.LocalName.Equals("DocumentTypes")
                                         ? rootElement
                                         : rootElement.Descendants("DocumentTypes").FirstOrDefault();

                if (docTypeElement != null)
                {
                    var contentTypes = packagingService.ImportContentTypes(docTypeElement);
                }


                //Stylesheets
                foreach (XmlNode n in packageXML.DocumentElement.SelectNodes("Stylesheets/Stylesheet"))
                {
                    //WB: Hopefully this gets a nice packagingService too...
                    StyleSheet s = StyleSheet.Import(n, currentUser);
                }
                


                //Documents
                var documentElement = rootElement.Descendants("DocumentSet").FirstOrDefault();
                if (documentElement != null)
                {
                    var content = packagingService.ImportContent(documentElement, -1);
                }


                //Package Actions
                foreach (XmlNode n in packageXML.DocumentElement.SelectNodes("Actions/Action"))
                {
                    if (n.Attributes["runat"] != null && n.Attributes["runat"].Value == "install")
                    {
                        try
                        {
                            umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(package.Id, n.Attributes["alias"].Value, n);
                        }
                        catch
                        {
                        }
                    }
                }
                
            }
        }

        public static void UninstallFromPackageXml(this IPackage package)
        {
             //Check to swee if we have a package.xml file in the root of the Contents folder
            var checkForPackageXML = package.GetContentFiles().SingleOrDefault(x => x.EffectivePath.EndsWith("\\package.xml"));

            //Check to see if we found the file
            if (checkForPackageXML != null)
            {
                //Found the package.xml file - let's use it

                //Actual path to xml file
                var mappedFileLocation = HostingEnvironment.MapPath("~/" + checkForPackageXML.EffectivePath);

                //Load the package.xml file into XDocument so we can use it with packaging service
                var packageXML = new XmlDocument();
                packageXML.Load(mappedFileLocation);
                

                //XElement - DataTypeDefinitions
                //Xml as XElement which is used with the new PackagingService
                var rootElement = packageXML.DocumentElement.GetXElement();

                //Run Uninstall Package Actions
                foreach (XmlNode n in packageXML.DocumentElement.SelectNodes("Actions/Action"))
                {
                    if (n.Attributes["runat"] != null && n.Attributes["runat"].Value == "install")
                    {
                        try
                        {
                            umbraco.cms.businesslogic.packager.PackageAction.UndoPackageAction(package.Id, n.Attributes["alias"].Value, n);
                        }
                        catch
                        {
                        }
                    }
                }

            }
        }

        //Taken from Umbraco.Core.XmlExtensions - But was an internal class :(
        public static XElement GetXElement(this XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

    }
}
