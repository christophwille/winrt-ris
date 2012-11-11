using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ris.Data.Models
{
    public class RisFulltextQueryParameter : RisQueryParameter
    {
        public string SearchText { get; set; }

        // For serialization
        public RisFulltextQueryParameter()
            : base()
        {
        }

        public RisFulltextQueryParameter(string searchText)
            : base()
        {
            SearchText = searchText;
        }

        public override string DisplayString
        {
            get { return SearchText; }
        }
    }
}
