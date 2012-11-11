using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Ris.Client.WinRT;
using Ris.Data.Models;
using Risotto.Models;
using Ris.Data;
using Ris.Client.Models;

namespace Risotto.ViewModels
{
    public class SearchResultsViewModel : RisViewModelBase
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

        public async Task SearchRisAsync()
        {
            UpdateInProgress = true;

            DocumentReferences = null;
            UpdateSearchResultInfo();

            var localQueryParam = QueryParameter;

            var result = await RisQueryWithIncrementalLoading.LoadPage(localQueryParam, 1);

            UpdateInProgress = false;

            if (result.Succeeded)
            {
                DocumentReferences = new RisQueryWithIncrementalLoading(localQueryParam, result,
                    IncrementalLoadingStarted, IncrementalLoadingCompleted, IncrementalLoadingFailed);
                UpdateSearchResultInfo();

                // TODO: Re-Enable storing once full advanced query is in place
                //var ctx = new RisDbContext();
                //ctx.InsertSearchHistoryEntry(new DbRisQueryParameter(localQueryParam, DocumentReferences.Hits));
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
                SearchResultInfo = String.Format("{0}: {1}", QueryParameter.DisplayString, message);
            }
            else if (null == DocumentReferences)
            {
                SearchResultInfo = QueryParameter.DisplayString;
            }
            else
            {
                SearchResultInfo = String.Format("{0} ({1} von {2} geladen)", QueryParameter.DisplayString, DocumentReferences.Count, DocumentReferences.Hits);
            }
        }

        public const string QueryParameterPropertyName = "QueryParameter";
        private RisQueryParameter _queryParameter = null;

        public RisQueryParameter QueryParameter
        {
            get
            {
                return _queryParameter;
            }
            set
            {
                Set(QueryParameterPropertyName, ref _queryParameter, value);
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
            if (null != state.QueryParameter)
            {
                QueryParameter = state.QueryParameter;
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
                                                QueryParameter, 
                                                resultTemp,
                                                IncrementalLoadingStarted, IncrementalLoadingCompleted, IncrementalLoadingFailed);

                UpdateSearchResultInfo();
            }
        }

        public SearchPageState SaveState()
        {
            var state = new SearchPageState()
            {
                SearchResultInfo = this.SearchResultInfo
            };

            if (null != QueryParameter)
            {
                state.QueryParameter = QueryParameter;
            }

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
