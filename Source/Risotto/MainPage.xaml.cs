﻿using System;
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

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            App.SearchHistoryChanged -= AppOnSearchHistoryChanged;

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
            throw new NotImplementedException();
        }

        private void AdvancedQueryButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate<AdvancedSearchPage>();
        }
    }
}
