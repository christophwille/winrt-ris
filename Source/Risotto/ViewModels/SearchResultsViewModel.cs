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
using Ris.Data;

namespace Risotto.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        public const string UpdateInProgressPropertyName = "UpdateInProgress";
        private bool _updateInProgress = false;

        public bool UpdateInProgress
        {
            get
            {
                return _updateInProgress;
            }
            set
            {
                Set(UpdateInProgressPropertyName, ref _updateInProgress, value);
            }
        }

        public const string DocumentReferencesPropertyName = "DocumentReferences";
        private RisQueryWithIncrementalLoading _documentReferences = null;

        public RisQueryWithIncrementalLoading DocumentReferences
        {
            get { return _documentReferences; }
            set
            {
                Set(DocumentReferencesPropertyName, ref _documentReferences, value);
            }
        }

        public async Task SearchRisAsync(string queryText)
        {
            UpdateInProgress = true;

            SearchText = queryText;
            DocumentReferences = null;
            UpdateSearchResultInfo();

            var queryParam = new RisFulltextQueryParameter(SearchText);
            var result = await RisQueryWithIncrementalLoading.LoadPage(queryParam, 1);

            UpdateInProgress = false;

            if (result.Succeeded)
            {
                DocumentReferences = new RisQueryWithIncrementalLoading(queryParam, result,
                    IncrementalLoadingStarted, IncrementalLoadingCompleted, IncrementalLoadingFailed);
                UpdateSearchResultInfo();

                var ctx = new RisDbContext();
                ctx.InsertSearchHistoryEntry(new DbRisQueryParameter(queryText, DocumentReferences.Hits));
            }
            else
            {
                UpdateSearchResultInfo("Abfrage fehlgeschlagen");
            }
        }
        
        private void IncrementalLoadingStarted()
        {
            UpdateInProgress = true;
        }

        private void IncrementalLoadingCompleted()
        {
            UpdateInProgress = false;
            UpdateSearchResultInfo();
        }

        private void IncrementalLoadingFailed(string message)
        {
            UpdateInProgress = false;
            UpdateSearchResultInfo(message);
        }

        private void UpdateSearchResultInfo(string message = null)
        {
            if (message != null)
            {
                SearchResultInfo = String.Format("{0}: {1}", SearchText, message);
            }
            else if (null == DocumentReferences)
            {
                SearchResultInfo = SearchText;
            }
            else
            {
                SearchResultInfo = String.Format("{0} ({1} von {2} geladen)", SearchText, DocumentReferences.Count, DocumentReferences.Hits);
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
            }
        }

        public const string SearchResultInfoPropertyName = "SearchResultInfo";
        private string _searchResultInfoMessage = "";

        public string SearchResultInfo
        {
            get
            {
                return _searchResultInfoMessage;
            }
            set
            {
                Set(SearchResultInfoPropertyName, ref _searchResultInfoMessage, value);
            }
        }

        public void LoadState(SearchPageState state)
        {
            if (!String.IsNullOrWhiteSpace(state.SearchText))
            {
                SearchText = state.SearchText;
            }

            if (!String.IsNullOrWhiteSpace(state.SearchResultInfo))
            {
                SearchResultInfo = state.SearchResultInfo;
            }

            if (null != state.DocumentReferencesList)
            {
                var resultTemp = new SearchResult()
                                     {
                                         Hits = state.Hits,
                                         Page = state.Page,
                                         PageSize = state.PageSize,
                                         DocumentReferences = state.DocumentReferencesList
                                     };

                DocumentReferences = new RisQueryWithIncrementalLoading(
                                                new RisFulltextQueryParameter(SearchText), 
                                                resultTemp,
                                                IncrementalLoadingStarted, IncrementalLoadingCompleted, IncrementalLoadingFailed);

                UpdateSearchResultInfo();
            }
        }

        public SearchPageState SaveState()
        {
            var state = new SearchPageState()
            {
                SearchText = this.SearchText,
                SearchResultInfo = this.SearchResultInfo
            };

            if (null != DocumentReferences)
            {
                state.DocumentReferencesList = DocumentReferences.ToList();
                state.Hits = DocumentReferences.Hits;
                state.Page = DocumentReferences.Page;
                state.PageSize = DocumentReferences.PageSize;
            }

            return state;
        }
    }
}
