using System;
using System.Collections.Generic;

namespace Ris.Client.Models
{
    public class SearchResult : ResultBase
    {
        public SearchResult(List<DocumentReference> docRefs, string page, string pageSize, string hits)
        {
            Succeeded = true;
            DocumentReferences = docRefs;

            int parsingResult = 0;
            if (Int32.TryParse(page, out parsingResult))
            {
                Page = parsingResult;
            }
            if (Int32.TryParse(pageSize, out parsingResult))
            {
                PageSize = parsingResult;
            }
            if (Int32.TryParse(hits, out parsingResult))
            {
                Hits = parsingResult;
            }
        }

        public SearchResult() : base()
        {
        }

        public SearchResult(string errorMessage) : base(errorMessage)
        {
        }

        public int? Page { get; set; }
        public int? Hits { get; set; }
        public int? PageSize { get; set; }

        public List<DocumentReference> DocumentReferences { get; set; }
    }
}
