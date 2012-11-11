using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Ris.Data;
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
using Risotto.Models;
using Ris.Data.Models;

namespace Risotto
{
    public sealed partial class AdvancedSearchPage : Risotto.Common.LayoutAwarePage
    {
        public AdvancedSearchViewModel ViewModel { get; set; }

        public AdvancedSearchPage()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new AdvancedSearchViewModel();
                DataContext = ViewModel;
            }
        }


        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (navigationParameter != null)
            {
                var param = SerializationHelper.DeserializeFromString<RisAdvancedQueryParameter>((string)navigationParameter);
                ViewModel.InitializeFromParameter(param);
            }

            if (pageState != null && pageState.ContainsKey(Constants.AdvancedSearchPageState))
            {
                string serializedState = pageState[Constants.AdvancedSearchPageState].ToString();
                var state = SerializationHelper.DeserializeFromString<AdvancedSearchPageState>(serializedState);

                ViewModel.LoadState(state);
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            string serializedState = SerializationHelper.SerializeToString(ViewModel.SaveState());
            pageState[Constants.AdvancedSearchPageState] = serializedState;
        }
    }
}
