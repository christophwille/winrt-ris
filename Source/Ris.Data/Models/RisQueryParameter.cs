using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ris.Data.Models
{
    [XmlInclude(typeof(RisFulltextQueryParameter))]
    [XmlInclude(typeof(RisAdvancedQueryParameter))]
    public class RisQueryParameter
    {
        public RisQueryParameter()
        {
            ImRisSeit = ChangedWithinEnum.Undefined;
        }

        public virtual string DisplayString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ChangedWithinEnum ImRisSeit { get; set; }
    }

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
    }
}
