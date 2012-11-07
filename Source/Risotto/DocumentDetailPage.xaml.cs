using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Risotto.Models;
using Risotto.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Risotto
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DocumentDetailPage : Risotto.Common.LayoutAwarePage, IWebViewFlyoutFixes
    {
        public DocumentDetailViewModel ViewModel { get; set; }

        public DocumentDetailPage()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new DocumentDetailViewModel();
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
            //var dataTransferManager = DataTransferManager.GetForCurrentView();
            //dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            var navParam = DocumentDetailNavigationParameter.FromNavigationParameter((string) navigationParameter);

            ViewModel.PageTitle = navParam.DokumentTitel;
            ViewModel.NavigationAction = navParam.Action;

            if (navParam.Action == NavigationAction.LoadFromUrl)
            {
                webView.Navigate(new Uri(navParam.Command));
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            //var dataTransferManager = DataTransferManager.GetForCurrentView();
            //dataTransferManager.DataRequested -= DataTransferManager_DataRequested;
        }

        void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            // TODO: The code below always returns nothing, thus the docs / sdk sample looks wrong
            // http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.datatransferpackage
            //
            if (NavigationAction.LoadFromUrl == ViewModel.NavigationAction)
            {
                DataPackage p = webView.DataTransferPackage;

                
                if (p.GetView().Contains(StandardDataFormats.Text))
                {
                    p.Properties.Title = ViewModel.PageTitle;
                    p.Properties.Description = "This is a snippet from the content hosted in the WebView control";
                    request.Data = p;
                }
                else
                {
                    request.FailWithDisplayText("Es gibt keine Inhalte die geteilt werden können");
                }
            }
        }

        public async Task OnFlyoutOpen()
        {
            await WebViewFlyoutFixes.FlyoutOpenAsync(webViewRect, webView);
        }

        public void OnFlyoutClose()
        {
            WebViewFlyoutFixes.FlyoutClose(webViewRect, webView);
        }
    }
}
