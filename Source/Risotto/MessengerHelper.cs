using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Risotto
{
    public static class MessengerHelper
    {
        public const string SearchHistoryDeleted = "SearchHistoryDeleted";
        public const string DownloadsDeleted = "DownloadsDeleted";

        public static void Notify(string message)
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("ignore"), message);
        }

        public static void Register(object target, string message, Action callback)
        {
            Messenger.Default.Register<NotificationMessage>(target, message, (msg) => callback());
        }

        public static void Unregister(object target, string message)
        {
            Messenger.Default.Unregister<NotificationMessage>(target, message);
        }
    }
}
