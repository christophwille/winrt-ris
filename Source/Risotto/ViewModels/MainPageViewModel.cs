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
        public const string DownloadedDocumentsPropertyName = "DownloadedDocuments";
        private List<DbDownloadedDocument> _downloads = null;
        public List<DbDownloadedDocument> DownloadedDocuments
        {
            get
            {
                return _downloads;
            }
            set
            {
                Set(DownloadedDocumentsPropertyName, ref _downloads, value);
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

        public const string SelectedSearchHistoryPropertyName = "SelectedSearchHistory";
        private DbRisQueryParameter _selectedHistoryItem = null;
        public DbRisQueryParameter SelectedSearchHistory
        {
            get { return _selectedHistoryItem; }
            set
            {
                Set(SelectedSearchHistoryPropertyName, ref _selectedHistoryItem, value);
                DeleteSelectedSearchHistoryItemCommand.RaiseCanExecuteChanged();
            }
        }

        public const string SelectedDownloadPropertyName = "SelectedDownload";
        private DbDownloadedDocument _selectedDownload = null;
        public DbDownloadedDocument SelectedDownload
        {
            get { return _selectedDownload; }
            set
            {
                Set(SelectedDownloadPropertyName, ref _selectedDownload, value);
                DeleteSelectedDownloadCommand.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand _deleteSelectedSearchHistoryItemCommand;
        public RelayCommand DeleteSelectedSearchHistoryItemCommand
        {
            get
            {
                return _deleteSelectedSearchHistoryItemCommand
                    ?? (_deleteSelectedSearchHistoryItemCommand = new RelayCommand(
                        () => DeleteSelectedSearchHistoryItemAsync(),
                        () => SelectedSearchHistory != null));
            }
        }

        private RelayCommand _deleteSelectedDownloadCommand;
        public RelayCommand DeleteSelectedDownloadCommand
        {
            get
            {
                return _deleteSelectedDownloadCommand
                    ?? (_deleteSelectedDownloadCommand = new RelayCommand(
                        () => DeleteSelectedDownloadAsync(),
                        () => SelectedDownload != null));
            }
        }

        public async Task DeleteSelectedSearchHistoryItemAsync()
        {
            var ctx = new RisDbContext();

            await ctx.DeleteSearchHistoryEntry(SelectedSearchHistory);
            await LoadSearchHistoryAsync();
        }

        public async Task DeleteSelectedDownloadAsync()
        {
            var ctx = new RisDbContext();

            await ctx.DeleteDownload(SelectedDownload);
            await LoadDownloadsAsync();
        }

        public async Task LoadSearchHistoryAsync()
        {
            var ctx = new RisDbContext();
            var history = await ctx.GetSearchHistoryEntries();

            SelectedSearchHistory = null;
            SearchHistory = history;

            if (null != _selectedSearchHistoryItemFromState && history != null)
            {
                SelectedSearchHistory = history.FirstOrDefault(h => h.Id == _selectedSearchHistoryItemFromState.Value);
            }

            if (history.Any())
            {
                MessengerHelper.Notify(MessengerHelper.DbLoadCompleted);
            }
        }

        public async Task LoadDownloadsAsync()
        {
            var ctx = new RisDbContext();
            var localCopies = await ctx.GetDownloads();

            SelectedDownload = null;
            DownloadedDocuments = localCopies;

            if (null != _selectedDownloadFromState && localCopies != null)
            {
                SelectedDownload = localCopies.FirstOrDefault(h => h.Id == _selectedDownloadFromState.Value);
            }

            if (localCopies.Any())
            {
                MessengerHelper.Notify(MessengerHelper.DbLoadCompleted);
            }
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

        private int? _selectedSearchHistoryItemFromState = null;
        private int? _selectedDownloadFromState = null;

        public void LoadState(MainPageState state)
        {
            if (!String.IsNullOrWhiteSpace(state.SearchText))
            {
                SearchText = state.SearchText;
            }

            _selectedSearchHistoryItemFromState = state.SelectedSearchHistoryItem;
            _selectedDownloadFromState = state.SelectedDownload;
        }

        public MainPageState SaveState()
        {
            var state = new MainPageState()
            {
                SearchText = this.SearchText,
            };

            if (null != SelectedSearchHistory)
            {
                state.SelectedSearchHistoryItem = SelectedSearchHistory.Id;
            }

            if (null != SelectedDownload)
            {
                state.SelectedDownload = SelectedDownload.Id;
            }

            return state;
        }
    }
}
