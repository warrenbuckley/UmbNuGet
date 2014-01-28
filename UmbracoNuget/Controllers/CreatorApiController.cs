using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using UmbracoNuget.Models;

namespace UmbracoNuget.Controllers
{
    [PluginController("NuGet")]
    public class CreatorApiController : UmbracoAuthorizedApiController
    {
        /// <summary>
        /// Used to get infro about the Umbraco install. List Macros, templates, doctypes 
        /// etc so user can pick & choose what to include package
        /// </summary>
        /// <returns></returns>
        public UmbracoInstallInfo GetUmbracoInfo()
        {
            var response = new UmbracoInstallInfo();

            //Content Types aka DocTypes
            response.ContentTypes = UmbracoContext.Application.Services.ContentTypeService.GetAllContentTypes();

            //Media Types (TODO: Can't serialise)
            //response.MediaTypes = UmbracoContext.Application.Services.ContentTypeService.GetAllMediaTypes();

            //Templates
            response.Templates = UmbracoContext.Application.Services.FileService.GetTemplates(); 

            //CSS - Only valid CSS files (as .txt file was getting picked up)
            response.Stylesheets = UmbracoContext.Application.Services.FileService.GetStylesheets().Where(x => x.IsFileValidCss());

            //Macros
            response.Macros = UmbracoContext.Application.Services.MacroService.GetAll();

            //Languages
            response.Languages = UmbracoContext.Application.Services.LocalizationService.GetAllLanguages();

            //Dictionary Items (TODO: Can't serialise)
            //response.DictionaryItems = UmbracoContext.Application.Services.LocalizationService.GetRootDictionaryItems();

            //Data Types 
            response.DataTypes = UmbracoContext.Application.Services.DataTypeService.GetAllDataTypeDefinitions();

            //Return the object
            return response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public FilePicker GetFilesInFolder(string folderPath = @"\")
        {

            //Get the folder we want to list files & folders
            var folderToOpen = HostingEnvironment.MapPath("~" + folderPath);

            //Go and get absolute path - so we can fetch files & folders off the disk
            var diskFolder = new DirectoryInfo(folderToOpen);

            //Site Path
            var sitePath = HostingEnvironment.MapPath("~").ToLower();

            //Get files in the folder
            var getFiles = diskFolder.GetFiles();

            //Use this list to store & add our model to a list
            var files = new List<Models.File>();

            //Loop over the files we found & hydrate our model
            foreach (var file in getFiles)
            {
                var fileToAdd       = new Models.File();
                fileToAdd.FileName  = file.Name;
                fileToAdd.FullPath  = file.FullName;
                fileToAdd.RelPath   = file.FullName.ToLower().Replace(sitePath, @"\");

                files.Add(fileToAdd);
            }

            //Get other folders in this folder
            var getFolders = diskFolder.GetDirectories();

            //Use this list to store & add our model to a list
            var folders = new List<Models.Folder>();

            //Loop over the files we found & hydrate our model
            foreach (var folder in getFolders)
            {
                var folderToAdd         = new Folder();
                folderToAdd.FolderName  = folder.Name;
                folderToAdd.FullPath    = folder.FullName;
                folderToAdd.RelPath     = folder.FullName.ToLower().Replace(sitePath, @"\");

                folders.Add(folderToAdd);
            }

            var response            = new FilePicker();
            response.CurrentFolder  = folderPath;
            response.Files          = files;
            response.Folders        = folders;

            return response;
        }

    }
}
