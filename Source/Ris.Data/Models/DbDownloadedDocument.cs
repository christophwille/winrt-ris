using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Ris.Data.Models
{
    public class DbDownloadedDocument
    {
        public DbDownloadedDocument()
        {
        }

        public DbDownloadedDocument(string nummer, string titel, string originalresultxml, string html)
        {
            DokumentNummer = nummer;
            DokumentTitel = titel;
            OriginalDocumentResultXml = originalresultxml;
            HtmlFromRisServer = html;

            LastDownloaded = DateTime.Now;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string DokumentNummer { get; set; }
        public string DokumentTitel { get; set; }

        // Serialized message that was returned from the server (for structured access to metadata)
        public string OriginalDocumentResultXml { get; set; }

        // Html without site layout that is retrieved from the server
        public string HtmlFromRisServer { get; set; }

        public DateTime LastDownloaded { get; set; }
    }
}
