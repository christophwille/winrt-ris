using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risotto.ViewModels
{
    public class AdvancedSearchViewModel : RisViewModelBase
    {
        public const string SuchwortePropertyName = "Suchworte";
        private string _suchworte = "";

        public string Suchworte
        {
            get
            {
                return _suchworte;
            }

            set { Set(SuchwortePropertyName, ref _suchworte, value); }
        }




        public void Reset()
        {
            Suchworte = "";
        }

        public void Submit()
        {
            // Step 1: Validate

            // Step 2: Transform

            // Step 3: Navigate
        }
    }
}
