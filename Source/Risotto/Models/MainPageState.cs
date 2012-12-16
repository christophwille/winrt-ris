using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data.Models;

namespace Risotto.Models
{
    public class MainPageState
    {
        public string SearchText { get; set; }

        public int? SelectedSearchHistoryItem { get; set; }
        public int? SelectedDownload { get; set; }

        // View State
        public double SearchHistoryVerticalOffset { get; set; }
        public double DownloadsVerticalOffset { get; set; }
    }
}
