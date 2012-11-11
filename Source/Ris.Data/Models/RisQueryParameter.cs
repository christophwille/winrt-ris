using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Ris.Client.Models;

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

    public class RisQueryParameterSerializeable
    {
        public RisQueryParameter QueryParameter { get; set; }

        public static string Serialize(RisQueryParameter p)
        {
            var toSerialize = new RisQueryParameterSerializeable()
                                  {
                                      QueryParameter = p
                                  };

            return SerializationHelper.SerializeToString(toSerialize);
        }

        public static RisQueryParameter Deserialize(string data)
        {
            var ds = SerializationHelper.DeserializeFromString<RisQueryParameterSerializeable>(data);

            return ds.QueryParameter;
        }
    }
}
