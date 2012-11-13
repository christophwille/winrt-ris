using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ris.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Risotto
{
    public sealed partial class PreferencesUserControl : UserControl
    {
        public PreferencesUserControl()
        {
            this.InitializeComponent();
        }

        private async void DeleteHistory_OnClick(object sender, RoutedEventArgs e)
        {
            deleteHistory.IsEnabled = false;

            var ctx = new RisDbContext();
            await ctx.DeleteSearchHistory();

            MessengerHelper.Notify(MessengerHelper.SearchHistoryDeleted);
        }

        private async void DeleteDownloads_OnClick(object sender, RoutedEventArgs e)
        {
            deleteDownloads.IsEnabled = false;

            var ctx = new RisDbContext();
            await ctx.DeleteDownloads();

            MessengerHelper.Notify(MessengerHelper.DownloadsDeleted);
        }
    }
}
