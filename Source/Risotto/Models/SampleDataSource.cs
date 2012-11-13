using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data.Models;
using Ris.Client.Models;

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

        private readonly List<DbDownloadedDocument> _downloads = new List<DbDownloadedDocument>();
        public List<DbDownloadedDocument> Downloads
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

            var dl = new DbDownloadedDocument()
                         {
                             Id = 1,
                             DokumentNummer = "NOR123455",
                             DokumentTitel = "§1 Faschingsgesetz",
                             LastDownloaded = DateTime.Now
                         };

            _downloads.Add(dl);

            _history.Add(new DbRisQueryParameter(new RisFulltextQueryParameter("Ehe"), 8787));
            _history.Add(new DbRisQueryParameter(new RisFulltextQueryParameter("Gesellschaftsrecht"), 46));
        }
    }
}
