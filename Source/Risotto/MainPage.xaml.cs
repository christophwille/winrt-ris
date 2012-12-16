using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Ris.Data;
using Risotto.Models;
using Risotto.ThirdParty;
using Risotto.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ris.Data.Models;
using Risotto.Services;

namespace Risotto
{
    public sealed partial class MainPage : Risotto.Common.LayoutAwarePage
    {
        public MainPageViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new MainPageViewModel();
                DataContext = ViewModel;
            }
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            MessengerHelper.Register(this, MessengerHelper.SearchHistoryDeleted, SearchHistoryDeleted);
            MessengerHelper.Register(this, MessengerHelper.DownloadsDeleted, DownloadsDeleted);

            ViewModel.LoadSearchHistoryAsync(); // intentionally no await
            ViewModel.LoadDownloadsAsync(); // intentionally no await

            if (pageState != null && pageState.ContainsKey(Constants.MainPageState))
            {
                string serializedState = pageState[Constants.MainPageState].ToString();
                var state = SerializationHelper.DeserializeFromString<MainPageState>(serializedState);

                ViewModel.LoadState(state);
            }
        }

        private void SearchHistoryDeleted()
        {
            ViewModel.LoadSearchHistoryAsync();
        }

        private void DownloadsDeleted()
        {
            ViewModel.LoadDownloadsAsync();
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            MessengerHelper.Unregister(this, MessengerHelper.SearchHistoryDeleted);
            MessengerHelper.Unregister(this, MessengerHelper.DownloadsDeleted);

            string serializedState = SerializationHelper.SerializeToString(ViewModel.SaveState());
            pageState[Constants.MainPageState] = serializedState;
        }

        private void SearchHistory_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DbRisQueryParameter;
            var query = item.RisQueryParameter;

            // Full text goes directly to search results, Advanced to the search definition page first
            if (query is RisFulltextQueryParameter)
            {
                string searchTextToPass = ((RisFulltextQueryParameter)item.RisQueryParameter).SearchText;
                NavigationService.Navigate<SearchResultsPage>(searchTextToPass);
            }
            else if (query is RisAdvancedQueryParameter)
            {
                string navParam = SerializationHelper.SerializeToString((RisAdvancedQueryParameter) query);
                NavigationService.Navigate<AdvancedSearchPage>(navParam);
            }
        }

        private void Downloads_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DbDownloadedDocument;

            var action = DocumentDetailNavigationParameter
                .CreateNavigationParameter(item.DokumentTitel, NavigationAction.LoadCachedDownload, item.Id.ToString());

            NavigationService.Navigate<DocumentDetailPage>(action);
        }

        private void AdvancedQueryButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate<AdvancedSearchPage>();
        }

        private void SearchText_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (ViewModel.CanSearchRis)
                {
                    ViewModel.SearchFulltext();
                }
            }
        }

        private void DownloadsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ManageAppBarOnSelection();
        }

        private void ManageAppBarOnSelection()
        {
            if (this.downloadsListView.SelectedItems.Count > 0)
            {
                BottomAppBar.IsSticky = true;
                BottomAppBar.IsOpen = true;
            }
            else
            {
                BottomAppBar.IsOpen = false;
                BottomAppBar.IsSticky = false;
            }
        }
    }
}
