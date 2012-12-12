using System.Diagnostics;
using System.Net.Http;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Ris.Client;
using Ris.Client.Messages;
using Ris.Client.Models;
using Ris.Client.WinRT;
using Ris.Data;
using Ris.Data.Models;
using Risotto.Models;

namespace Risotto.ViewModels
{
    public class DocumentDetailViewModel : RisViewModelBase
    {
        public bool IsDebugBuild
        {
            get
            {
#if DEBUG
                return true;
#endif
                return false;
            }
        }
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

        public const string SourceHtmlPropertyName = "SourceHtml";
        private string _sourceHtml = null;

        public string SourceHtml
        {
            get
            {
                return _sourceHtml;
            }
            set
            {
                Set(SourceHtmlPropertyName, ref _sourceHtml, value);
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
                RaisePropertyChanged(CanAddDownloadPropertyName);
            }
        }

        public const string CurrentDocumentPropertyName = "CurrentDocument";
        private Ris.Client.Models.DocumentResult _currentDocument = null;

        public Ris.Client.Models.DocumentResult CurrentDocument
        {
            get
            {
                return _currentDocument;
            }
            set
            {
                Set(CurrentDocumentPropertyName, ref _currentDocument, value);
            }
        }

        public const string AttachmentsPropertyName = "Attachments";
        private List<Ris.Client.Models.DocumentContent> _attachments = null;

        public List<Ris.Client.Models.DocumentContent> Attachments
        {
            get
            {
                return _attachments;
            }
            set
            {
                Set(AttachmentsPropertyName, ref _attachments, value);
            }
        }

        public async Task Load()
        {
            UpdateInProgress = true;
            PageTitle = "Lade " + NavigationParameter.DokumentTitel;

            bool loadingSucceeded = false;

            if (NavigationParameter.Action == NavigationAction.LoadFromService)
            {
                var result = await Task.Run(() => ParallelLoadSynced());

                if (null != result)
                {
                    CurrentDocument = result.Item1;
                    SourceHtml = result.Item2;
                    loadingSucceeded = true;
                }
            }
            else if (NavigationParameter.Action == NavigationAction.LoadCachedDownload)
            {
                loadingSucceeded = await LoadFromCacheAsync();
            }

            if (!loadingSucceeded)
            {
                PageTitle = "Fehler: Laden fehlgeschlagen";

                CurrentDocument = null;
                Attachments = null;
                SourceHtml = null;
            }
            else
            {
                PageTitle = CreateTitleFromDocument();
                Attachments = CurrentDocument.GetAttachments();
            }

            RaisePropertyChanged(CanAddDownloadPropertyName);
            UpdateInProgress = false;
        }

        private Tuple<DocumentResult, string> ParallelLoadSynced()
        {
            var svcTask = new Task<DocumentResult>(() => LoadFromServiceAsync().Result);
            var htmlTask = new Task<string>(() => DownloadHtmlFromRisServer().Result);

            svcTask.Start();
            htmlTask.Start();

            Task.WaitAll(svcTask, htmlTask);

            DocumentResult docResult = svcTask.Result;
            string html = htmlTask.Result;

            if (null != docResult && html != null)
            {
                return new Tuple<DocumentResult, string>(docResult, html);
            }

            return null;
        }

        private async Task<string> DownloadHtmlFromRisServer()
        {
            try
            {
                string url = RisUrlHelper.UrlForHtmlFromDokumentNummer(NavigationParameter.Command);

                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    client.Dispose();

                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string CreateTitleFromDocument()
        {
            var item = CurrentDocument.Document;
            return String.Format("{0} {1}", item.ArtikelParagraphAnlage, item.Kurztitel);
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
                SourceHtml = doc.HtmlFromRisServer;

                CachedDocumentDatabaseId = doc.Id;

                return CurrentDocument.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadFromCacheAsync::" + ex.ToString());
            }

            return false;
        }

        private async Task<DocumentResult> LoadFromServiceAsync()
        {
            var client = new RisClient();
            var result = await client.GetDocumentAsync(NavigationParameter.Command);

            if (result.Succeeded)
            {
                return result;
            }

            return null;
        }

        private RelayCommand _addDownloadCommand;
        public RelayCommand AddDownloadCommand
        {
            get
            {
                return _addDownloadCommand
                    ?? (_addDownloadCommand = new RelayCommand(
                        async () => await AddDownloadAsync(), () => CanAddDownload));
            }
        }

        private async Task AddDownloadAsync()
        {
            try
            {
                var ctx = new RisDbContext();

                var dl = new DbDownloadedDocument(NavigationParameter.Command,
                                                  NavigationParameter.DokumentTitel,
                                                  CurrentDocument.OriginalDocumentResultXml,
                                                  SourceHtml);

                await ctx.InsertDownload(dl);

                _addOperationHasBeenExecuted = true;
                CachedDocumentDatabaseId = dl.Id;

                RaisePropertyChanged(CanAddDownloadPropertyName);
            }
            catch (Exception)
            {
            }
        }

        public const string CanAddDownloadPropertyName = "CanAddDownload";
        public bool CanAddDownload
        {
            get
            {
                if (null == NavigationParameter) return false;
                if (NavigationParameter.Action == NavigationAction.LoadFromUrl) return false;
                if (NavigationParameter.Action == NavigationAction.LoadCachedDownload) return false;

                if (CurrentDocument != null && CurrentDocument.Succeeded && !_addOperationHasBeenExecuted)
                    return true;

                return false;
            }
        }

        private bool _addOperationHasBeenExecuted = false;

        public const string CachedDocumentDatabaseIdPropertyName = "CachedDocumentDatabaseId";
        private int? _cachedDocumentDbId = null;

        public int? CachedDocumentDatabaseId
        {
            get
            {
                return _cachedDocumentDbId;
            }
            set
            {
                Set(CachedDocumentDatabaseIdPropertyName, ref _cachedDocumentDbId, value);
            }
        }

        private RelayCommand _refreshCachedDocumentCommand;
        public RelayCommand RefreshCachedDocumentCommand
        {
            get
            {
                return _refreshCachedDocumentCommand
                    ?? (_refreshCachedDocumentCommand = new RelayCommand(
                        async () => await RefreshCachedDocumentAsync(), () => CachedDocumentDatabaseId != null));
            }
        }

        private async Task RefreshCachedDocumentAsync()
        {
            // #1: Refresh the data
            // #2: Delete old row and Insert in one transaction
        }
    }
}
