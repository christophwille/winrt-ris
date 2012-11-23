using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ris.Client.Models
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

        public byte[] Content { get; set; }

        public string ProposedFilename
        {
            get { return Name + "." + Mapper.MapDocumentContentDataTypeEnumToExtension(DataType); }
        }
    }
}
