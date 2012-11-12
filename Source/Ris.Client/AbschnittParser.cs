using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ris.Client.Models;
using Req = Ris.Client.Messages.Request;

namespace Ris.Client
{
    public class AbschnittParser
    {
        public static Req.NormabschnittSucheinschraenkung Parse(string von, string bis, AbschnittTypEnum abschnitt)
        {
            von = von.Trim();
            bis = bis.Trim();

            if (String.IsNullOrWhiteSpace(von) && String.IsNullOrWhiteSpace(bis))
                return null;

            int nummerVon, nummerBis = 0;
            string buchstabeVon = "", buchstabeBis = "";
            bool parseBisOk = true;
            bool didParseBis = false;

            if (!String.IsNullOrWhiteSpace(bis))
            {
                parseBisOk = Parse(bis, out nummerBis, out buchstabeBis);
                didParseBis = true;
            }

            if (Parse(von, out nummerVon, out buchstabeVon) && parseBisOk)
            {
                var req =  new Req.NormabschnittSucheinschraenkung()
                           {
                               NummerVon = nummerVon.ToString(),
                               BuchstabeVon = buchstabeVon,
                               BuchstabeBis = buchstabeBis,
                               Typ = Mapper.MapAbschnittTypToNormabschnittTyp(abschnitt)
                           };

                if (didParseBis)
                {
                    req.NummerBis = nummerBis;
                    req.NummerBisSpecified = true;
                }

                return req;
            }

            return null;
        }

        // Format: (optional) Numeric followed by any number of spaces (including none) + character(s)
        public static bool Parse(string input, out int nummer, out string buchstabe)
        {
            input = input.Trim();
            int nInputLength = input.Length;
            bool parsedNummer = false;

            nummer = 0;
            buchstabe = "";

            string firstPart = "";
            for (int i = 0; i < nInputLength; i++)
            {
                char current = input[i];
                if (Char.IsDigit(current))
                {
                    firstPart += current;
                }
                else
                {
                    break;
                }
            }

            int nLength = firstPart.Length;
            if (nLength > 0)
            {
                nummer = Int32.Parse(firstPart);
                parsedNummer = true;
            }

            if (nLength < nInputLength)
            {
                buchstabe = input.Substring(nLength).Trim();
            }

            return (parsedNummer);
        }
    }
}
