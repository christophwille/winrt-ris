using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.ToastContent;
using Windows.UI.Notifications;

namespace Risotto.Services
{
    public static class ToastService
    {
        public static void Display(string heading, string body)
        {
            IToastText02 templateContent = ToastContentFactory.CreateToastText02();
            templateContent.TextHeading.Text = heading;
            templateContent.TextBodyWrap.Text = body;

            IToastNotificationContent toastContent = templateContent;

            toastContent.Duration = ToastDuration.Short;
            ToastNotification toast = toastContent.CreateNotification();

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
