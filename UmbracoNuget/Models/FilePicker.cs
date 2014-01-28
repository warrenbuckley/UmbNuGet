using System.Collections.Generic;
using System.IO;

namespace UmbracoNuget.Models
{
    public class FilePicker
    {
        public IEnumerable<File> Files { get; set; }

        public IEnumerable<Folder> Folders { get; set; } 

        public string CurrentFolder { get; set; }
    }

    public class File
    {
        // cms.dll
        public string FileName { get; set; }

        // C:\inetpub\wwwroot\Personal\UmbracoNuGet\UmbracoNuget\UmbracoNuget.NightlySite\bin\cms.dll
        public string FullPath { get; set; }

        // \bin\cms.dll
        public string RelPath { get; set; }
    }

    public class Folder
    {
        // x86
        public string FolderName { get; set; }

        // C:\inetpub\wwwroot\Personal\UmbracoNuGet\UmbracoNuget\UmbracoNuget.NightlySite\bin\x86
        public string FullPath { get; set; }

        // \bin\x86
        public string RelPath { get; set; }
    }

}
