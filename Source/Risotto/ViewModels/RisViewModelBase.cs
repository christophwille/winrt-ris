using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MetroLog;
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

        public RisViewModelBase()
        {
            CreateLogger = () => LogManagerFactory.DefaultLogManager.GetLogger(GetType(), null);
        }

        public Func<ILogger> CreateLogger { get; set; }
        private ILogger _logger = null;

        public ILogger Log { 
            get 
            { 
                if (null != _logger) return _logger;

                _logger = CreateLogger();
                return _logger;
            }
        }
    }
}
