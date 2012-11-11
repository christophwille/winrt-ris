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
using Ris.Client.Models;

namespace Risotto.ViewModels
{
    public class AdvancedSearchViewModel : RisViewModelBase
    {
        public AdvancedSearchViewModel()
        {
            // Set the search defaults
            FassungVom = DateTime.Now.ToString("d");

            ImRisSeitSource = ImRisSeitListItem.GenerateList();
            SelectImRisSeitListItem(ChangedWithinEnum.Undefined);

            Kundmachungsorgane = Kundmachungsorgan.GenerateList();
            SelectKundmachungsorgan("");
        }

        public List<ImRisSeitListItem> ImRisSeitSource { get; private set; }
        public List<Kundmachungsorgan> Kundmachungsorgane { get; set; }

        public const string SelectedImRisSeitListItemPropertyName = "SelectedImRisSeitListItem";
        private ImRisSeitListItem _selectedImRisSeitListItem = null;

        public ImRisSeitListItem SelectedImRisSeitListItem
        {
            get
            {
                return _selectedImRisSeitListItem;
            }

            set { Set(SelectedImRisSeitListItemPropertyName, ref _selectedImRisSeitListItem, value); }
        }

        public const string SelectedKundmachungsorganPropertyName = "SelectedKundmachungsorgan";
        private Kundmachungsorgan _selectedKundmachungsorgan = null;

        public Kundmachungsorgan SelectedKundmachungsorgan
        {
            get
            {
                return _selectedKundmachungsorgan;
            }

            set { Set(SelectedKundmachungsorganPropertyName, ref _selectedKundmachungsorgan, value); }
        }

        private void SelectImRisSeitListItem(ChangedWithinEnum c)
        {
            SelectedImRisSeitListItem = ImRisSeitSource.FirstOrDefault(i => i.ImRisSeit == c);
        }

        private void SelectKundmachungsorgan(string k)
        {
            SelectedKundmachungsorgan = Kundmachungsorgane.FirstOrDefault(i => i.Text == k);
        }

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

        public const string TitelAbkuerzungPropertyName = "TitelAbkuerzung";
        private string _titelakue = "";

        public string TitelAbkuerzung
        {
            get
            {
                return _titelakue;
            }

            set { Set(TitelAbkuerzungPropertyName, ref _titelakue, value); }
        }

        public const string ParagrafVonPropertyName = "ParagrafVon";
        private string _paragrafvon = "";

        public string ParagrafVon
        {
            get
            {
                return _paragrafvon;
            }

            set { Set(ParagrafVonPropertyName, ref _paragrafvon, value); }
        }

        public const string ParagrafBisPropertyName = "ParagrafBis";
        private string _paragrafbis = "";

        public string ParagrafBis
        {
            get
            {
                return _paragrafbis;
            }

            set { Set(ParagrafBisPropertyName, ref _paragrafbis, value); }
        }

        public const string ArtikelVonPropertyName = "ArtikelVon";
        private string _artikelvon = "";

        public string ArtikelVon
        {
            get
            {
                return _artikelvon;
            }

            set { Set(ArtikelVonPropertyName, ref _artikelvon, value); }
        }

        public const string ArtikelBisPropertyName = "ArtikelBis";
        private string _artikelbis = "";

        public string ArtikelBis
        {
            get
            {
                return _artikelbis;
            }

            set { Set(ArtikelBisPropertyName, ref _artikelbis, value); }
        }

        public const string AnlageVonPropertyName = "AnlageVon";
        private string _anlagevon = "";

        public string AnlageVon
        {
            get
            {
                return _anlagevon;
            }

            set { Set(AnlageVonPropertyName, ref _anlagevon, value); }
        }

        public const string AnlageBisPropertyName = "AnlageBis";
        private string _anlagebis = "";

        public string AnlageBis
        {
            get
            {
                return _anlagebis;
            }

            set { Set(AnlageBisPropertyName, ref _anlagebis, value); }
        }

        public const string KundmachungsorganNummerPropertyName = "KundmachungsorganNummer";
        private string _kundmachungsorganNummer = "";

        public string KundmachungsorganNummer
        {
            get
            {
                return _kundmachungsorganNummer;
            }

            set { Set(KundmachungsorganNummerPropertyName, ref _kundmachungsorganNummer, value); }
        }

        public const string TypPropertyName = "Typ";
        private string _typ = "";

        public string Typ
        {
            get
            {
                return _typ;
            }

            set { Set(TypPropertyName, ref _typ, value); }
        }

        public const string IndexPropertyName = "Index";
        private string _index = "";

        public string Index
        {
            get
            {
                return _index;
            }

            set { Set(IndexPropertyName, ref _index, value); }
        }

        public const string UnterzeichnungsdatumPropertyName = "Unterzeichnungsdatum";
        private string _unterzeichnungsdatum = "";

        public string Unterzeichnungsdatum
        {
            get
            {
                return _unterzeichnungsdatum;
            }

            set { Set(UnterzeichnungsdatumPropertyName, ref _unterzeichnungsdatum, value); }
        }

        public const string FassungVomPropertyName = "FassungVom";
        private string _fassungvom = "";

        public string FassungVom
        {
            get
            {
                return _fassungvom;
            }

            set { Set(FassungVomPropertyName, ref _fassungvom, value); }
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
