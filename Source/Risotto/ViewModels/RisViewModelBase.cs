using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Risotto.Services;

namespace Risotto.ViewModels
{
    public class RisViewModelBase : ViewModelBase
    {
        private RelayCommand _homeCommand;
        public RelayCommand HomeCommand
        {
            get
            {
                return _homeCommand
                       ?? (_homeCommand = new RelayCommand(GoHome));
            }
        }

        private void GoHome()
        {
            NavigationService.Navigate<MainPage>();
        }
    }
}
