using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.Extensions;

namespace Risotto
{
    //
    // Adapted from: http://blogs.msdn.com/b/priozersk/archive/2012/09/09/how-to-restore-scroll-position-of-the-gridview-when-navigating-back.aspx
    //
    public static class ScrollViewerHelpers
    {
        private static ScrollViewer GetScrollViewer(Control parent)
        {
            return parent.GetFirstDescendantOfType<ScrollViewer>();
        }

        public static double GetHorizontalOffset(Control ctrl)
        {
            return GetScrollViewer(ctrl).HorizontalOffset;
        }

        public static void ScrollToHorizontalOffset(Control ctrl, double offset)
        {
            GetScrollViewer(ctrl).ScrollToHorizontalOffset(offset);
        }
    }
}
