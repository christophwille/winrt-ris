using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ris.Data.Models
{
    [KnownType(typeof(RisFulltextQueryParameter))]
    [KnownType(typeof(RisAdvancedQueryParameter))]
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
