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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Risotto.Services;

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
            var searchText = navigationParameter as String;

            // Do we return from a detail page?
            if (pageState != null && pageState.ContainsKey(Constants.SearchResultsPageState))
            {
                string serializedState = pageState[Constants.SearchResultsPageState].ToString();
                var state = SerializationHelper.DeserializeFromString<SearchPageState>(serializedState);

                ViewModel.LoadState(state);
            }
            else
            {
                ViewModel.SearchRisAsync(searchText);
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            string serializedState = SerializationHelper.SerializeToString(ViewModel.SaveState());
            pageState[Constants.SearchResultsPageState] = serializedState;
        }

        private void ItemsControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DocumentReference;
            var title = String.Format("{0} {1}", item.ArtikelParagraphAnlage, item.Kurzinformation);

            var action = DocumentDetailNavigationParameter
                .CreateNavigationParameter(title, NavigationAction.LoadFromUrl, item.DokumentUrl);

            NavigationService.Navigate<DocumentDetailPage>(action);
        }
    }
}
