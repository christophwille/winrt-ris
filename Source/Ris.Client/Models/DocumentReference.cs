using System;

namespace Ris.Client.Models
{
    public class DocumentReference
    {
        public string Dokumentnummer { get; set; }
        public string Kurzinformation { get; set; }
        public string DokumentUrl { get; set; }
        public string ArtikelParagraphAnlage { get; set; }
        public string Applikation { get; set; }

        //
        // Fix for http://78.41.145.171/Dokument.wxe?Abfrage=Bundesnormen&Dokumentnummer=NOR12110323
        //
        public static string FixDocumentUrl(string url)
        {
            int startIndex = url.IndexOf("//") + 2;
            int endIndex = url.IndexOf('/', startIndex);
            return url.Replace(url.Substring(startIndex, endIndex - startIndex), "www.ris.bka.gv.at");
        }
    }
}
