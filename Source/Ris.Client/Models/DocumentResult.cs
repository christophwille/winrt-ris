using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ris.Client.Models
{
    public class DocumentResult : ResultBase
    {
        public DocumentResult()
            : base()
        {
        }

        public DocumentResult(string errorMessage)
            : base(errorMessage)
        {
        }

        public Document Document { get; set; }
        public List<DocumentContent> DocumentContents { get; set; }
        public string OriginalDocumentResultXml { get; set; }
    }
}
