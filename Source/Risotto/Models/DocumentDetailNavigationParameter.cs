using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data;

namespace Risotto.Models
{
    public enum NavigationAction
    {
        LoadCachedDownload,
        LoadFromService,
        LoadFromUrl
    }

    public class DocumentDetailNavigationParameter
    {
        public DocumentDetailNavigationParameter()
        {
        }

        public DocumentDetailNavigationParameter(string dokumentTitle, NavigationAction action, string command)
        {
            Action = action;
            Command = command;
            DokumentTitel = dokumentTitle;
        }

        public NavigationAction Action { get; set; }
        public string Command { get; set; }
        public string DokumentTitel { get; set; }

        public static string CreateNavigationParameter(string dokumentTitle, NavigationAction action, string command)
        {
            var parameter = new DocumentDetailNavigationParameter(dokumentTitle, action, command);
            return SerializationHelper.SerializeToString(parameter);
        }

        public static DocumentDetailNavigationParameter FromNavigationParameter(string navParam)
        {
            return SerializationHelper.DeserializeFromString<DocumentDetailNavigationParameter>(navParam);
        }
    }
}
