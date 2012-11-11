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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Risotto
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
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
