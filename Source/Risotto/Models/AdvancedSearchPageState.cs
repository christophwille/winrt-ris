using Ris.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risotto.Models
{
    public class AdvancedSearchPageState
    {
        public string Suchworte { get; set; }

        public string TitelAbkuerzung { get; set; }

        public string Index { get; set; }

        public string Typ { get; set; }

        public string Unterzeichnungsdatum { get; set; }

        public string FassungVom { get; set; }

        public string ParagrafVon { get; set; }

        public string ParagrafBis { get; set; }

        public string ArtikelVon { get; set; }

        public string ArtikelBis { get; set; }

        public string AnlageVon { get; set; }

        public string AnlageBis { get; set; }

        public string Kundmachungsorgan { get; set; }

        public string KundmachungsorganNummer { get; set; }

        public ChangedWithinEnum ImRisSeit { get; set; }
    }
}
