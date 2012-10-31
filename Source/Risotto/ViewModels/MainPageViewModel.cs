using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Ris.Client;
using Ris.Data.Models;
using Risotto.Models;
using Risotto.Services;
using Ris.Data;

namespace Risotto.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public const string DownloadsPropertyName = "Downloads";
        private List<Download> _downloads = null;
        public List<Download> Downloads
        {
            get
            {
                return _downloads;
            }
            set
            {
                Set(DownloadsPropertyName, ref _downloads, value);
            }
        }

        public const string SearchHistoryPropertyName = "SearchHistory";
        private List<DbRisQueryParameter> _history = null;
        public List<DbRisQueryParameter> SearchHistory
        {
            get
            {
                return _history;
            }
            set
            {
                Set(SearchHistoryPropertyName, ref _history, value);
            }
        }

        public async Task LoadSearchHistoryAsync()
        {
            var ctx = new RisDbContext();
            var history = await ctx.GetTenMostRecentSearchHistoryEntries();

            SearchHistory = history;
        }

        private RelayCommand _searchRisCommand;
        public RelayCommand FulltextSearchCommand
        {
            get
            {
                return _searchRisCommand
                       ?? (_searchRisCommand = new RelayCommand(
                                                   SearchFulltext,
                                                   () => CanSearchRis));
            }
        }

        public void SearchFulltext()
        {
            NavigationService.Navigate<SearchResultsPage>(SearchText);
        }

        public bool CanSearchRis
        {
            get
            {
                return SearchText.Trim().Length >= 3;
            }
        }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                Set(SearchTextPropertyName, ref _searchText, value);
                FulltextSearchCommand.RaiseCanExecuteChanged();
            }
        }

        // UpdateSourceTrigger HACK
        public Action<string> UpdateBoundSearchTextProperty
        {
            get { return new Action<string>((value) => SearchText = value); }
        }

        public void LoadState(MainPageState state)
        {
            if (!String.IsNullOrWhiteSpace(state.SearchText))
            {
                SearchText = state.SearchText;
            }
        }

        public MainPageState SaveState()
        {
            var state = new MainPageState()
            {
                SearchText = this.SearchText,
            };

            return state;
        }
    }
}
