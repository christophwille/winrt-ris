using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ris.Client.Messages
{
    public static class MessageSerializationHelper
    {
        public static T DeserializeFromString<T>(string data)
        {
            using (var stringReader = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }

        // Documents beginning with <?xml version="1.0" encoding="utf-16"?> isn't fine with the parser on the server-side (it seems)
        public static string SerializeToString<T>(T value)
        {
            var settings = new XmlWriterSettings()
                               {
                                   OmitXmlDeclaration = true
                               };
#if DEBUG
            settings.Indent = true;
#endif

            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, value);

                    ms.Position = 0;
                    string result = new StreamReader(ms).ReadToEnd();
                    return result;
                }
            }
        }
    }
}
