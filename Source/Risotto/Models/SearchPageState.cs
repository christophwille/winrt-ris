using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data.Models;

namespace Risotto.Models
{
    public class SearchPageState
    {
        public string SearchText { get; set; }
        public string SearchResultInfo { get; set; }

        public int? Hits { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public List<DocumentReference> DocumentReferencesList { get; set; }
    }
}
