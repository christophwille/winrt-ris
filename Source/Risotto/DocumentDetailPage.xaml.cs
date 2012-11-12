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

namespace Risotto
{
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

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            var navParam = DocumentDetailNavigationParameter.FromNavigationParameter((string) navigationParameter);

            ViewModel.PageTitle = navParam.DokumentTitel;
            ViewModel.NavigationAction = navParam.Action;

            if (navParam.Action == NavigationAction.LoadFromUrl)
            {
                webView.Navigate(new Uri(navParam.Command));
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= DataTransferManager_DataRequested;
        }

        //
        // http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.datatransferpackage
        // didn't work, thus DataRequested uses code from
        // http://codesnack.com/blog/2012/01/05/metro-webview-source-workarounds/
        //
        void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            if (NavigationAction.LoadFromUrl == ViewModel.NavigationAction)
            {
                try
                {
                    string html = webView.InvokeScript("eval", new string[] { "document.documentElement.outerHTML;" });
                    
                    // TODO: fix Urls to base Urls otherwise it won't look right (css, js et cetera missing)

                    request.Data.Properties.Title = ViewModel.PageTitle;
                    request.Data.SetHtmlFormat(html);
                }
                catch
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
