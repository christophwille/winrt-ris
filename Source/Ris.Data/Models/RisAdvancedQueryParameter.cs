using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ris.Data.Models
{
    public class RisAdvancedQueryParameter : RisQueryParameter
    {
        // For serialization
        public RisAdvancedQueryParameter()
            : base()
        {
        }

        public override string DisplayString
        {
            get { return "NOT IMPLEMENTED"; }
        }

        public string Suchworte { get; set; }
    }
}
