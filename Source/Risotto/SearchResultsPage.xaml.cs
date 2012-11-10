using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Ris.Data;
using Ris.Data.Models;
using Risotto.Models;
using Risotto.ViewModels;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Risotto.Services;
using Ris.Client.Models;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace Risotto
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage : Risotto.Common.LayoutAwarePage
    {
        public SearchResultsViewModel ViewModel { get; set; }

        public SearchResultsPage()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new SearchResultsViewModel();
                DataContext = ViewModel;
            }
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            ViewModel.QueryParameter = NavigationParameterToSearchParameter(navigationParameter as String);

            // Do we return from a detail page?
            if (pageState != null && pageState.ContainsKey(Constants.SearchResultsPageState))
            {
                string serializedState = pageState[Constants.SearchResultsPageState].ToString();
                var state = SerializationHelper.DeserializeFromString<SearchPageState>(serializedState);

                ViewModel.LoadState(state);
            }
            else
            {
                ViewModel.SearchRisAsync();
            }
        }

        //
        // This conversion is necessary because we could be called from the Search charm
        //
        private RisQueryParameter NavigationParameterToSearchParameter(string s)
        {
            try
            {
                var p = SerializationHelper.DeserializeFromString<RisQueryParameter>(s);
                return p;
            }
            catch (Exception)
            {
                // If it cannot be deserialized, we assume it to be a fulltext search
                return new RisFulltextQueryParameter(s);
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            string serializedState = SerializationHelper.SerializeToString(ViewModel.SaveState());
            pageState[Constants.SearchResultsPageState] = serializedState;
        }

        private void ItemsControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            string currentVisualState = DetermineVisualState(ApplicationView.Value);
            var item = e.ClickedItem as DocumentReference;

            if (0 == String.Compare("Snapped", currentVisualState, StringComparison.CurrentCultureIgnoreCase))
            {
                // If we are in Snapped View, always open the browser side-by-side (more efficient for reading)
                Windows.System.Launcher.LaunchUriAsync(new Uri(item.DokumentUrl));
            }
            else
            {
                var title = String.Format("{0} {1}", item.ArtikelParagraphAnlage, item.Kurzinformation);

                var action = DocumentDetailNavigationParameter
                    .CreateNavigationParameter(title, NavigationAction.LoadFromUrl, item.DokumentUrl);

                NavigationService.Navigate<DocumentDetailPage>(action);
            }
        }
    }
}
