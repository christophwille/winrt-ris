using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ris.Data.Models
{
    public class DocumentContent
    {
        public string Name { get; set; }

        public DocumentContentTypeEnum ContentType { get; set; }
        public DocumentContentDataTypeEnum DataType { get; set; }

        public string Nutzdaten { get; set; }

        public bool IsHauptdokument()
        {
            return ContentType == DocumentContentTypeEnum.MainDocument;
        }
    }
}
