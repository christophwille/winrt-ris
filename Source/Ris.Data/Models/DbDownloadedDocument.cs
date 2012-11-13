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

        public DbDownloadedDocument(string nummer, string titel, string originalresultxml)
        {
            DokumentNummer = nummer;
            DokumentTitel = DokumentTitel;
            OriginalDocumentResultXml = originalresultxml;

            LastDownloaded = DateTime.Now;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string DokumentNummer { get; set; }
        public string DokumentTitel { get; set; }

        public string OriginalDocumentResultXml { get; set; }

        public DateTime LastDownloaded { get; set; }
    }
}
