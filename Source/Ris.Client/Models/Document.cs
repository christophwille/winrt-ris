using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ris.Client.Models
{
    public class Document
    {
        public string Abkuerzung { get; set; }
        public string Aenderung { get; set; }
        public DateTime? Aenderungsdatum { get; set; }
        public string AlteDokumentnummer { get; set; }
        public string Anmerkung { get; set; }
        public string AnmerkungZurGanzenRechtsvorschrift { get; set; }
        public string ArtikelParagraphAnlage { get; set; }
        public DateTime? Ausserkrafttretedatum { get; set; }
        public string Beachte { get; set; }
        public string BeachteZurGanzenRechtsvorschrift { get; set; }
        public string Dokumentnummer { get; set; }
        public string Gesetzesnummer { get; set; }
        public List<string> Indizes { get; set; }
        public DateTime? Inkrafttretedatum { get; set; }
        public string Kundmachungsorgan { get; set; }
        public string Kurztitel { get; set; }
        public string Langtitel { get; set; }
        public string Schlagworte { get; set; }
        public string Sprachen { get; set; }
        public string Staaten { get; set; }
        public string Typ { get; set; }
        public string Uebergangsrecht { get; set; }
        public DateTime? Unterzeichnungsdatum { get; set; }
        public DateTime? Veroeffentlichungsdatum { get; set; }

        public string OGDWebDocument { get; set; }
    }
}
