using GalaSoft.MvvmLight;
using Ris.Client.PhraseParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data;
using Ris.Data.Models;
using Risotto.Models;
using Risotto.Services;

namespace Risotto.ViewModels
{
    public class AdvancedSearchViewModel : RisViewModelBase
    {
        public AdvancedSearchViewModel()
        {
            ImRisSeitSource = ImRisSeitListItem.GenerateList();
        }

      public List<ImRisSeitListItem> ImRisSeitSource { get; private set; } 

        public const string ValidationMessagePropertyName = "ValidationMessage";
        private string _validationMessage = "";

        public string ValidationMessage
        {
            get
            {
                return _validationMessage;
            }

            set { Set(ValidationMessagePropertyName, ref _validationMessage, value); }
        }


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

            // TODO: Add all fields to this reset list
        }

        private bool Validate()
        {
            ValidateNonEmptyTextWithParser(Suchworte, "Suchworte enthält keine gültige Abfrage");

            // TODO: Validate all fields

            return String.IsNullOrWhiteSpace(ValidationMessage);
        }

        private bool ValidateNonEmptyTextWithParser(string searchText, string validationMessageToAdd)
        {
            if (String.IsNullOrWhiteSpace(searchText))
                return true;

            try
            {
                var expr = QueryParser.Parse(searchText);
                return true;
            }
            catch (ParseException)
            {
                ValidationMessage += validationMessageToAdd + Environment.NewLine;
                return false;
            }
        }

        public void Submit()
        {
            if (!Validate()) return;

            var p = new RisAdvancedQueryParameter()
                        {
                            Suchworte = this.Suchworte
                            // TODO: Add other fields
                        };

            string navParam = RisQueryParameterSerializeable.Serialize(p);
            NavigationService.Navigate<SearchResultsPage>(navParam);
        }
    }
}
