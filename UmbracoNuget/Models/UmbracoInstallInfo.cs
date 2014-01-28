using System.Collections.Generic;
using Umbraco.Core.Models;

namespace UmbracoNuget.Models
{
    public class UmbracoInstallInfo
    {
        public IEnumerable<IMacro> Macros { get; set; }

        public IEnumerable<IDataTypeDefinition> DataTypes { get; set; }

        public IEnumerable<IContentType> ContentTypes { get; set; }

        public IEnumerable<IMediaType> MediaTypes { get; set; }

        public IEnumerable<ITemplate> Templates { get; set; }

        public IEnumerable<Stylesheet> Stylesheets { get; set; }

        public IEnumerable<ILanguage> Languages { get; set; }

        public IEnumerable<IDictionaryItem> DictionaryItems { get; set; } 
    }
}
