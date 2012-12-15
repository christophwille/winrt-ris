using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risotto.Models
{
    public class DocumentDetailPageState
    {
        public bool AddOperationHasBeenExecuted { get; set; }
        public int? CachedDocumentDatabaseId { get; set; }
    }
}
