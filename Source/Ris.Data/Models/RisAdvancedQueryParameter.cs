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
        }

        public override string DisplayString
        {
            get { return "NOT IMPLEMENTED"; }
        }

        public string Suchworte { get; set; }

        public string TitelAbkuerzung { get; set; }

        public string Index { get; set; }

        public string Typ { get; set; }

        public DateTime? FassungVom { get; set; }

        public DateTime? Unterzeichnungsdatum { get; set; }

        public int? ParagrafVon { get; set; }

        public int? ParagrafBis { get; set; }

        public string Kundmachungsorgan { get; set; }

        public string ArtikelVon { get; set; }

        public string ArtikelBis { get; set; }

        public string AnlageVon { get; set; }

        public string AnlageBis { get; set; }

        public string KundmachungsorganNummer { get; set; }
    }
}
