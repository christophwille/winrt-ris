using System.Diagnostics;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Client;
using Ris.Client.Messages;
using Ris.Client.Models;
using Ris.Client.WinRT;
using Ris.Data;
using Risotto.Models;

namespace Risotto.ViewModels
{
    public class DocumentDetailViewModel : RisViewModelBase
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

        public const string NavigationParameterPropertyName = "NavigationParameter";
        private DocumentDetailNavigationParameter _navigationParameter = null;

        public DocumentDetailNavigationParameter NavigationParameter
        {
            get
            {
                return _navigationParameter;
            }
            set
            {
                Set(NavigationParameterPropertyName, ref _navigationParameter, value);
            }
        }

        private Ris.Client.Models.DocumentResult CurrentDocument { get; set; }

        public async Task Load()
        {
            UpdateInProgress = true;
            PageTitle = "Lade " + NavigationParameter.DokumentTitel;

            bool loadingSucceeded = false;

            if (NavigationParameter.Action == NavigationAction.LoadFromService)
            {
                loadingSucceeded = await LoadFromServiceAsync();
            }
            else if (NavigationParameter.Action == NavigationAction.LoadCachedDownload)
            {
                loadingSucceeded = await LoadFromCacheAsync();
            }

            if (!loadingSucceeded)
            {
                PageTitle = "Fehler: Laden fehlgeschlagen";
                CurrentDocument = null;
            }
            else
            {
                PageTitle = NavigationParameter.DokumentTitel;

                // TODO: Xslt processing for displaying the Html content
            }

            UpdateInProgress = false;
        }

        private async Task<bool> LoadFromCacheAsync()
        {
            int id = 0;
            if (!Int32.TryParse(NavigationParameter.Command, out id)) return false;

            var ctx = new RisDbContext();
            var doc = await ctx.GetDownload(id);

            if (null == doc) return false;

            try
            {
                // Rehydrate document from storage
                var documentResult = MessageSerializationHelper.DeserializeFromString<Ris.Client.Messages.Document.DocumentResult>(doc.OriginalDocumentResultXml);
                CurrentDocument = Mapper.MapDocumentResult(documentResult);

                return CurrentDocument.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadFromCacheAsync::" + ex.ToString());
            }
            
            return false;
        }

        private async Task<bool> LoadFromServiceAsync()
        {
            var client = new RisClient();
            var result = await client.GetDocumentAsync(NavigationParameter.Command);

            if (result.Succeeded)
            {
                CurrentDocument = result;
                return true;
            }

            return false;
        }
    }
}
