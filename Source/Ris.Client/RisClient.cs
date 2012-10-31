using Ris.Client.Messages;
using Ris.Data.Models;

using Req = Ris.Client.Messages.Request;
using Resp = Ris.Client.Messages.Response;
using Doc = Ris.Client.Messages.Document;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ris.Client
{
    // This wraps all the calls to the Web Service / nice frontend with proper classes
    public class RisClient
    {
        public const string ErrorOnVersionRequest = "Versionsabfrage RIS OGD Service fehlgeschlagen";
        public Func<OGDServiceSoapClient> CreateServiceClient { get; set; } 

        public RisClient()
        {
            CreateServiceClient = () => new OGDServiceSoapClient();
        }

        public async Task<string> GetVersion()
        {
            var client = CreateServiceClient();
            string versionToReturn = ErrorOnVersionRequest;

            try
            {
                var version = await client.versionAsync();
                versionToReturn = version.Body.versionResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RisClient::GetVersion Exception: " + ex.ToString());
            }

            return versionToReturn;
        }

        private Req.T_OGDSearchRequest PrepareFulltextSearch(RisFulltextQueryParameter param)
        {
            var request = new Req.T_OGDSearchRequest();
            var volltext = new Req.PhraseSearchExpression();

            volltext.Value = param.SearchText;
            request.Suchworte = volltext;

            return request;
        }

        public async Task<SearchResult> QueryAsync(RisQueryParameter param, int seitenNummer)
        {
            Req.T_OGDSearchRequest request = null;

            if (param is RisFulltextQueryParameter)
            {
                request = PrepareFulltextSearch((RisFulltextQueryParameter)param);
            }

            if (null == request)
                return new SearchResult("Kein Query Processor gefunden");

            return await QueryAsync(request, seitenNummer);
        }

        private async Task<SearchResult> QueryAsync(Req.T_OGDSearchRequest request, int seitenNummer)
        {
            request.Seitennummer = seitenNummer;
            request.Sortierung = new Req.BundesnormenSortExpression()
            {
                SortDirection = Req.WebSortDirection.Ascending,
                SortedByColumn = Req.BundesnormenSortableColumn.Kurzinformation
            };

            request.DokumenteProSeiteSpecified = true;
            request.DokumenteProSeite = Req.PageSize.Fifty;

            request.ImRisSeitSpecified = true;
            request.ImRisSeit = Req.ChangeSetInterval.Undefined;

            try
            {
                string requestAsString = MessageSerializationHelper.SerializeToString(request);

                var client = CreateServiceClient();
                requestResponse response = await client.requestAsync("Br", requestAsString);

                var searchResult = MessageSerializationHelper.DeserializeFromString<Resp.T_OGDSearchResult>(response.Body.requestResult);

                if (searchResult.status == Resp.T_OGDSearchResultStatus.error)
                {
                    var error = (Resp.T_Error)searchResult.Item;
                    return new SearchResult(error.Message);
                }
                else
                {
                    var documentsResult = (Resp.T_OGDSearchResultSearchDocumentsResult)searchResult.Item;

                    var mappedDocumentReferences = documentsResult.DocumentReferences
                        .Select(dr => new DocumentReference
                                        {
                                            Dokumentnummer = dr.Dokumentnummer,
                                            DokumentUrl = DocumentReference.FixDocumentUrl(dr.DokumentUrl),
                                            Kurzinformation = dr.Kurzinformation,
                                            ArtikelParagraphAnlage = dr.ArtikelParagraphAnlage,
                                            Applikation = dr.Applikation.ToString()
                                        })
                                        .ToList();

                    return new SearchResult(mappedDocumentReferences, 
                                                documentsResult.Hits.pageNumber,
                                                documentsResult.Hits.pageSize, 
                                                documentsResult.Hits.Value);
                }
            }
            catch (Exception ex)
            {
                return new SearchResult(ex.ToString());
            }
        }

        public async Task<DocumentResult> GetDocument(string dokumentNummer)
        {
            var client = CreateServiceClient();

            try
            {
                getDocumentResponse doc = await client.getDocumentAsync("Br", dokumentNummer);

                var documentResult = MessageSerializationHelper.DeserializeFromString<Doc.DocumentResult>(doc.Body.getDocumentResult);

                if (documentResult.status == Doc.DocumentResultStatus.error)
                {
                    var error = (Doc.T_Error)documentResult.Item;
                    return new DocumentResult(error.Message);
                }
                else
                {
                    var documentsResult = (Doc.T_OGDWebDocument)documentResult.Item;

                    // TODO: Transform to Document

                    foreach (Doc.T_WebDocumentContentReference content in documentsResult.Dokumentinhalt)
                    {
                        // .Item could be: risdok or base64 (eg attachments)
                        string ct = content.ContentType.ToString();

                        if (content.Item is Doc.risdok)
                        {
                            var risdok = (Doc.risdok)content.Item;

                            if (null != risdok.nutzdaten)
                            {
                                // var forXsltProcessing = risdok.nutzdaten.Text;
                            }
                        }
                        else
                        {
                            
                        }

                        // TODO: Transform to DocumentContent
                    }

                    return new DocumentResult("not implemented");
                }
            }
            catch (Exception ex)
            {
                return new DocumentResult(ex.ToString());
            }
        }
    }
}
