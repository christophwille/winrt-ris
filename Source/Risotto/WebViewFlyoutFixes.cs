using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Callisto.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Risotto
{
    //
    // http://cggallant.blogspot.co.at/2012/09/windows-8-windows-store-apps-and.html
    // http://chriskoenig.net/2012/10/30/webview-vs-settingsflyout/
    //

    public interface IWebViewFlyoutFixes
    {
        Task OnFlyoutOpen();
        void OnFlyoutClose();
    }

    public static class WebViewFlyoutFixes
    {
        public static void ShowSettingsFlyout(SettingsFlyout flyout)
        {
            var detailPage = GetCurrentContentPage();
            if (null != detailPage)
            {
                FixWebViewZOrderProblem(flyout, detailPage);
            }

            flyout.IsOpen = true;
        }

        public async static void ShowFlyout(Flyout flyout, DocumentDetailPage page)
        {
            flyout.Closed += (s, e) => page.OnFlyoutClose();
            await page.OnFlyoutOpen();
            flyout.IsOpen = true;
        }

        private static IWebViewFlyoutFixes GetCurrentContentPage()
        {
            var currentWindow = Window.Current;

            // Safety checks
            if (null == currentWindow) return null;
            if (null == currentWindow.Content) return null;
            if (!(currentWindow.Content is Frame)) return null;

            return ((Frame)currentWindow.Content).Content as IWebViewFlyoutFixes;
        }

        private static void FixWebViewZOrderProblem(SettingsFlyout flyout, IWebViewFlyoutFixes page)
        {
            flyout.Closed += (s, e) => page.OnFlyoutClose();
            page.OnFlyoutOpen();
        }

        public static async Task FlyoutOpenAsync(Rectangle webViewRect, WebView webView)
        {
            var b = new WebViewBrush();
            b.SourceName = webView.Name;
            b.Redraw();

            webViewRect.Fill = b;

            await Task.Delay(100);

            webView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        public static void FlyoutClose(Rectangle webViewRect, WebView webView)
        {
            webView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            webViewRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }
    }
}
