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
                if (!String.IsNullOrWhiteSpace(Suchworte)) return Suchworte;
                if (!String.IsNullOrWhiteSpace(TitelAbkuerzung)) return TitelAbkuerzung;
                if (FassungVom.HasValue) return FassungVom.Value.ToString("d");
                
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
