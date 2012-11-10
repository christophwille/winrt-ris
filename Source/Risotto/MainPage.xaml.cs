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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Risotto
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
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

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            App.SearchHistoryChanged += AppOnSearchHistoryChanged;

            ViewModel.LoadSearchHistoryAsync(); // intentionally no await

            if (pageState != null && pageState.ContainsKey(Constants.MainPageState))
            {
                string serializedState = pageState[Constants.MainPageState].ToString();
                var state = SerializationHelper.DeserializeFromString<MainPageState>(serializedState);

                ViewModel.LoadState(state);
            }
        }

        private void AppOnSearchHistoryChanged(object sender, EventArgs eventArgs)
        {
            ViewModel.LoadSearchHistoryAsync();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            App.SearchHistoryChanged -= AppOnSearchHistoryChanged;

            string serializedState = SerializationHelper.SerializeToString(ViewModel.SaveState());
            pageState[Constants.MainPageState] = serializedState;
        }

        private void SearchHistory_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DbRisQueryParameter;
            string searchTextToPass = ((RisFulltextQueryParameter) item.RisQueryParameter).SearchText;

            // TODO: switch to serialization when advanced query is implemented, currently direct cast to full text search (see above)
            NavigationService.Navigate<SearchResultsPage>(searchTextToPass);
        }

        private void Downloads_OnItemClick(object sender, ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AdvancedQueryButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate<AdvancedSearchPage>();
        }
    }
}
