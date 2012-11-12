using Ris.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ris.Data.Models
{
    public class RisAdvancedQueryParameter : RisQueryParameter
    {
        // For serialization
        public RisAdvancedQueryParameter()
            : base()
        {
            AbschnittTyp = AbschnittTypEnum.NotSpecifiedInQuery;
        }

        public override string DisplayString
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Suchworte)) return "Suchworte: " + Suchworte;
                if (!String.IsNullOrWhiteSpace(TitelAbkuerzung)) return "Titel, Abkürzung: " + TitelAbkuerzung;
                if (AbschnittTyp != AbschnittTypEnum.NotSpecifiedInQuery)
                {
                    return AbschnittTyp.ToString() + " von " + Von + " bis " + Bis;
                }
                if (FassungVom.HasValue) return "Fassung vom: " + FassungVom.Value.ToString("d");
                
                return "Erweiterte Abfrage";
            }
        }

        public string Suchworte { get; set; }
        public string TitelAbkuerzung { get; set; }

        public string Index { get; set; }
        public string Typ { get; set; }

        public DateTime? Unterzeichnungsdatum { get; set; }
        public DateTime? FassungVom { get; set; }

        public string Von { get; set; }
        public string Bis { get; set; }
        public AbschnittTypEnum AbschnittTyp { get; set; }

        public string Kundmachungsorgan { get; set; }
        public string KundmachungsorganNummer { get; set; }
    }
}
