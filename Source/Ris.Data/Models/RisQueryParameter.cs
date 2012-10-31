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
    public abstract class RisQueryParameter
    {
        public virtual string DisplayString
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public class RisFulltextQueryParameter : RisQueryParameter
    {
        public string SearchText { get; set; }

        public RisFulltextQueryParameter(string searchText)
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
        public override string DisplayString
        {
            get { return "NOT IMPLEMENTED"; }
        }
    }
}
