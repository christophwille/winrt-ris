using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risotto.ViewModels
{
    public class DocumentDetailViewModel : ViewModelBase
    {
        public const string UpdateInProgressPropertyName = "UpdateInProgress";
        private bool _updateInProgress = false;

        public bool UpdateInProgress
        {
            get
            {
                return _updateInProgress;
            }
            set
            {
                Set(UpdateInProgressPropertyName, ref _updateInProgress, value);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                Set(PageTitlePropertyName, ref _pageTitle, value);
            }
        }
    }
}
