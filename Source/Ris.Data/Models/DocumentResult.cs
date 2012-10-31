using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ris.Data.Models
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
    }
}
