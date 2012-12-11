using System.Text;
using System.Threading.Tasks;
using Callisto.Controls;
using GalaSoft.MvvmLight;
using Ris.Client;
using Ris.Client.Models;
using Risotto.Models;
using Risotto.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
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

            var navParam = DocumentDetailNavigationParameter.FromNavigationParameter((string)navigationParameter);
            ViewModel.NavigationParameter = navParam;

            if (navParam.Action == NavigationAction.LoadFromUrl)
            {
                ViewModel.PageTitle = navParam.DokumentTitel;
                webView.Navigate(new Uri(navParam.Command));
            }
            else
            {
                ViewModel.Load();
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= DataTransferManager_DataRequested;

            // TODO: save state
        }

        //
        // http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.datatransferpackage
        // didn't work, thus DataRequested uses code from
        // http://codesnack.com/blog/2012/01/05/metro-webview-source-workarounds/
        //
        void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            if (NavigationAction.LoadFromUrl == ViewModel.NavigationParameter.Action)
            {
                ProvideSharingDataFromWebView(request);
            }
            else
            {
                if (ViewModel.SourceHtml != null)
                {
                    request.Data.Properties.Title = ViewModel.PageTitle;
                    request.Data.SetHtmlFormat(ViewModel.SourceHtml);
                }
                else
                {
                    request.FailWithDisplayText("Es gibt keine Inhalte die geteilt werden können");
                }
            }
        }

        private void ProvideSharingDataFromWebView(DataRequest request)
        {
            try
            {
                var html = new StringBuilder(webView.InvokeScript("eval", new string[] { "document.documentElement.outerHTML;" }));

                // Fix Urls to base Urls otherwise it won't look right (css, js et cetera missing) - really, really simple 
                html.Replace("src=\"/", "src=\"http://www.ris.bka.gv.at/");
                html.Replace("href=\"/", "href=\"http://www.ris.bka.gv.at/");

                request.Data.Properties.Title = ViewModel.PageTitle;
                request.Data.SetHtmlFormat(html.ToString());
            }
            catch
            {
                request.FailWithDisplayText("Es gibt keine Inhalte die geteilt werden können");
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

        private async void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("Html Datei", new List<string> { ".html" });
            fileSavePicker.DefaultFileExtension = ".html";

            string dokumentNummer = ViewModel.CurrentDocument.Document.Dokumentnummer;

            fileSavePicker.SuggestedFileName = String.Format("Ris{0}.html", dokumentNummer);

            var fileToSave = await fileSavePicker.PickSaveFileAsync();
            if (null == fileToSave) return;

            using (var stream = await fileToSave.OpenStreamForWriteAsync())
            {
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(ViewModel.SourceHtml);
                await writer.FlushAsync();
                writer.Dispose();
            }
        }

        private void ViewAttachments_OnClick(object sender, RoutedEventArgs e)
        {
            var f = new Flyout()
                           {
                               Placement = PlacementMode.Top,
                               PlacementTarget = (UIElement)sender
                           };

            Menu m = new Menu();

            foreach (var attachment in ViewModel.Attachments)
            {
                var mi = new MenuItem()
                             {
                                 Text = attachment.ProposedFilename,
                                 Tag = attachment
                             };

                mi.Tapped += Attachment_OnTapped;

                m.Items.Add(mi);
            }

            f.Content = m;
            WebViewFlyoutFixes.ShowFlyout(f, this);
        }

        private async void Attachment_OnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            var mi = sender as MenuItem;
            var attachment = mi.Tag as DocumentContent;

            if (attachment == null || attachment.Content == null) return;

            var extension = Mapper.MapDocumentContentDataTypeEnumToExtension(attachment.DataType);

            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add(extension + " Datei", new List<string> { "." + extension });
            fileSavePicker.DefaultFileExtension = "." + extension;

            fileSavePicker.SuggestedFileName = attachment.ProposedFilename;

            var fileToSave = await fileSavePicker.PickSaveFileAsync();
            if (null == fileToSave) return;

            using (var stream = await fileToSave.OpenStreamForWriteAsync())
            {
                await stream.WriteAsync(attachment.Content, 0, attachment.Content.Length);
                await stream.FlushAsync();
                stream.Dispose();

                Windows.System.Launcher.LaunchFileAsync(fileToSave);
            }
        }

        private async void DevXml_OnClick(object sender, RoutedEventArgs e)
        {
            if (null == ViewModel.CurrentDocument) return;

            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("Service XML", new List<string> { ".xml" });
            fileSavePicker.DefaultFileExtension = ".xml";

            string dokumentNummer = ViewModel.CurrentDocument.Document.Dokumentnummer;

            fileSavePicker.SuggestedFileName = String.Format("Ris{0}.xml", dokumentNummer);

            var fileToSave = await fileSavePicker.PickSaveFileAsync();
            if (null == fileToSave) return;

            using (var stream = await fileToSave.OpenStreamForWriteAsync())
            {
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(ViewModel.CurrentDocument.OriginalDocumentResultXml);
                await writer.FlushAsync();
                writer.Dispose();
            }

            // bool launched = await Windows.System.Launcher.LaunchFileAsync(fileToSave);
        }

        private void ViewOnWeb_OnClick(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri(ViewModel.CurrentDocument.Document.HtmlUrl));
        }

        private void ViewPdf_OnClick(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri(ViewModel.CurrentDocument.Document.PdfUrl));
        }
    }
}
