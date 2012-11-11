using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Ris.Client.Models;

namespace Ris.Data.Models
{
    public class DbRisQueryParameter
    {
        public DbRisQueryParameter()
        {
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Executed { get; set; }
        public int? Hits { get; set; }
        public ChangedWithinEnum ImRisSeit { get; set; }

        // RisFulltextQueryParameter
        public string FulltextSearchString { get; set; }

        public DbRisQueryParameter(RisQueryParameter qp, int? hits)
        {
            if (qp is RisFulltextQueryParameter)
            {
                FulltextSearchString = ((RisFulltextQueryParameter)qp).SearchText;
            }
            else
            {
                var adv = (RisAdvancedQueryParameter)qp;
                Suchworte = adv.Suchworte;
                TitelAbkuerzung = adv.TitelAbkuerzung;
                ParagrafVon = adv.ParagrafVon;
                ParagrafBis = adv.ParagrafBis;
                ArtikelVon = adv.ArtikelVon;
                ArtikelBis = adv.ArtikelBis;
                AnlageVon = adv.AnlageVon;
                AnlageBis = adv.AnlageBis;
                Kundmachungsorgan = adv.Kundmachungsorgan;
                KundmachungsorganNummer = adv.KundmachungsorganNummer;
                Typ = adv.Typ;
                Index = adv.Index;
                Unterzeichnungsdatum = adv.Unterzeichnungsdatum;
                FassungVom = adv.FassungVom;
            }

            Hits = hits;
            Executed = DateTime.Now;
            ImRisSeit = qp.ImRisSeit;
        }

        private RisQueryParameter _transformed = null;

        [Ignore]
        public RisQueryParameter RisQueryParameter
        {
            get
            {
                if (null != _transformed) return _transformed;

                if (!String.IsNullOrWhiteSpace(FulltextSearchString))
                {
                    _transformed = new RisFulltextQueryParameter(FulltextSearchString);
                }
                else
                {
                    _transformed = new RisAdvancedQueryParameter()
                                  {
                                      Suchworte = this.Suchworte,
                                      TitelAbkuerzung = this.TitelAbkuerzung,
                                      ParagrafVon = this.ParagrafVon,
                                      ParagrafBis = this.ParagrafBis,
                                      ArtikelVon = this.ArtikelVon,
                                      ArtikelBis = this.ArtikelBis,
                                      AnlageVon = this.AnlageVon,
                                      AnlageBis = this.AnlageBis,
                                      Kundmachungsorgan = this.Kundmachungsorgan,
                                      KundmachungsorganNummer = this.KundmachungsorganNummer,
                                      Typ = this.Typ,
                                      Index = this.Index,
                                      Unterzeichnungsdatum = this.Unterzeichnungsdatum,
                                      FassungVom = this.FassungVom,
                                  };
                }

                _transformed.ImRisSeit = this.ImRisSeit;
                return _transformed;
            }
        }

        [Ignore]
        public string SearchInformation
        {
            get { return String.Format("{0} Resultate am {1:d}", Hits, Executed); }
        }

        // Advanced Search
        public string Suchworte { get; set; }
        public string TitelAbkuerzung { get; set; }
        public int? ParagrafVon { get; set; }
        public int? ParagrafBis { get; set; }
        public string ArtikelVon { get; set; }
        public string ArtikelBis { get; set; }
        public string AnlageVon { get; set; }
        public string AnlageBis { get; set; }
        public string Kundmachungsorgan { get; set; }
        public string KundmachungsorganNummer { get; set; }
        public string Typ { get; set; }
        public string Index { get; set; }
        public DateTime? Unterzeichnungsdatum { get; set; }
        public DateTime? FassungVom { get; set; }
    }
}
