using Ris.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risotto.Models
{
    public class ImRisSeitListItem
    {
        public ImRisSeitListItem(ChangedWithinEnum c, string t)
        {
            ImRisSeit = c;
            Text = t;
        }

        public ChangedWithinEnum ImRisSeit { get; set; }
        public string Text { get; set; }

        public static List<ImRisSeitListItem> GenerateList()
        {
            var list = new List<ImRisSeitListItem>();

            list.Add(new ImRisSeitListItem(ChangedWithinEnum.Undefined, ""));
            list.Add(new ImRisSeitListItem(ChangedWithinEnum.EinerWoche, "einer Woche"));
            list.Add(new ImRisSeitListItem(ChangedWithinEnum.ZweiWochen, "zwei Wochen"));
            list.Add(new ImRisSeitListItem(ChangedWithinEnum.EinemMonat, "einem Monat"));
            list.Add(new ImRisSeitListItem(ChangedWithinEnum.DreiMonaten, "drei Monaten"));
            list.Add(new ImRisSeitListItem(ChangedWithinEnum.SechsMonaten, "sechs Monaten"));
            list.Add(new ImRisSeitListItem(ChangedWithinEnum.EinemJahr, "einem Jahr"));

            return list;
        }
    }
}
