using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ris.Client
{
    public static class RisUrlHelper
    {
        private static string UrlFromDokumentNummer(string dokumentNummer, string extension)
        {
            return String.Format("http://www.ris.bka.gv.at/Dokumente/Bundesnormen/{0}/{0}.{1}", dokumentNummer, extension);
        }

        public static string UrlForPdfFromDokumentNummer(string dokumentNummer)
        {
            return UrlFromDokumentNummer(dokumentNummer, "pdf");
        }

        public static string UrlForHtmlFromDokumentNummer(string dokumentNummer)
        {
            return UrlFromDokumentNummer(dokumentNummer, "html");
        }

        public static string UrlForRtfFromDokumentNummer(string dokumentNummer)
        {
            return UrlFromDokumentNummer(dokumentNummer, "rtf");
        }
    }
}
