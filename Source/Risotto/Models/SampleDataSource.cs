using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data.Models;

namespace Risotto.Models
{
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private readonly List<DocumentReference> _documentReferences = new List<DocumentReference>();
        public List<DocumentReference> DocumentReferences
        {
            get { return _documentReferences; }
        }

        private readonly List<Download> _downloads = new List<Download>();
        public List<Download> Downloads
        {
            get { return _downloads; }
        }

        private readonly List<DbRisQueryParameter> _history = new List<DbRisQueryParameter>();
        public List<DbRisQueryParameter> SearchHistory
        {
            get { return _history; }
        }

        public SampleDataSource()
        {
            var dm = new DocumentReference()
            {
                Dokumentnummer = "NOR123455",
                DokumentUrl = "http://www.orf.at/",
                Kurzinformation = "Nix",
                ArtikelParagraphAnlage = "Artikel sowieso"
            };

            _documentReferences.Add(dm);

            var dl = new Download()
                         {
                             Name = "Fake Sample Download"
                         };

            _downloads.Add(dl);

            _history.Add(new DbRisQueryParameter("Ehe", 8787));
            _history.Add(new DbRisQueryParameter("Gesellschaftsrecht", 46));
        }
    }
}
