using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risotto.Models
{
    public class Kundmachungsorgan
    {
        public Kundmachungsorgan(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        // Copied from the Web site as the service does not expose this as a list
        public static List<Kundmachungsorgan> GenerateList()
        {
            var list = new List<Kundmachungsorgan>();

            list.Add(new Kundmachungsorgan(""));
            list.Add(new Kundmachungsorgan("BGBl. I Nr."));
            list.Add(new Kundmachungsorgan("BGBl. II Nr."));
            list.Add(new Kundmachungsorgan("BGBl. III Nr."));
            list.Add(new Kundmachungsorgan("BGBl. Nr."));
            list.Add(new Kundmachungsorgan("RGBl. Nr."));
            list.Add(new Kundmachungsorgan("StGBl. Nr."));
            list.Add(new Kundmachungsorgan("AmtlNHW Nr."));
            list.Add(new Kundmachungsorgan("DJ S"));
            list.Add(new Kundmachungsorgan("DRAnz. Nr."));
            list.Add(new Kundmachungsorgan("dRGBl. I S"));
            list.Add(new Kundmachungsorgan("dRGBl. II S"));
            list.Add(new Kundmachungsorgan("dRGBl. S"));
            list.Add(new Kundmachungsorgan("GBlÖ Nr."));
            list.Add(new Kundmachungsorgan("GVBlTirVbg.Nr."));
            list.Add(new Kundmachungsorgan("JABl. Nr."));
            list.Add(new Kundmachungsorgan("JakschGL II, S"));
            list.Add(new Kundmachungsorgan("JGS Nr."));
            list.Add(new Kundmachungsorgan("JMVBl. Nr."));
            list.Add(new Kundmachungsorgan("JosGS II., Nr."));
            list.Add(new Kundmachungsorgan("LGBlBgld. Nr."));
            list.Add(new Kundmachungsorgan("LGBlKtn. Nr."));
            list.Add(new Kundmachungsorgan("LGBlOÖ Nr."));
            list.Add(new Kundmachungsorgan("LGBlTir. Nr."));
            list.Add(new Kundmachungsorgan("LGBlVbg. Nr."));
            list.Add(new Kundmachungsorgan("LGVBlSbg. Nr."));
            list.Add(new Kundmachungsorgan("LGVBlTir. Nr."));
            list.Add(new Kundmachungsorgan("MBl. I S"));
            list.Add(new Kundmachungsorgan("MThGS Bd. 6, Nr."));
            list.Add(new Kundmachungsorgan("MThGS Bd. 7, Nr."));
            list.Add(new Kundmachungsorgan("NSlgpolVerwD Nr."));
            list.Add(new Kundmachungsorgan("PGS Nr."));
            list.Add(new Kundmachungsorgan("PTVBl. Nr."));
            list.Add(new Kundmachungsorgan("RMinBl. S"));
            list.Add(new Kundmachungsorgan("RVBl. Nr."));
            list.Add(new Kundmachungsorgan("SlgGOeudEns14"));
            list.Add(new Kundmachungsorgan("VABlNiederdonau S"));
            list.Add(new Kundmachungsorgan("VABlWien Nr."));
            list.Add(new Kundmachungsorgan("VerBKAVVers. Nr."));
            list.Add(new Kundmachungsorgan("Zl. II b Nr."));

            return list;
        }
    }
}
