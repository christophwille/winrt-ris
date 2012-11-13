using Ris.Client.Messages;
using Ris.Data.Models;
using Ris.Client.Models;

using Req = Ris.Client.Messages.Request;
using Resp = Ris.Client.Messages.Response;
using Doc = Ris.Client.Messages.Document;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Client.PhraseParser;

namespace Ris.Client.WinRT
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

        public async Task<string> GetVersionAsync()
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
                Debug.WriteLine("RisClient::GetVersionAsync Exception: " + ex.ToString());
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

        private Req.T_OGDSearchRequest PrepareAdvancedSearch(RisAdvancedQueryParameter param)
        {
            var request = new Req.T_OGDSearchRequest();

            if (!String.IsNullOrWhiteSpace(param.Suchworte))
            {
                request.Suchworte = QueryParser.Parse(param.Suchworte);
            }

            if (!String.IsNullOrWhiteSpace(param.TitelAbkuerzung))
            {
                request.Titel = QueryParser.Parse(param.TitelAbkuerzung);
            }

            if (param.AbschnittTyp != AbschnittTypEnum.NotSpecifiedInQuery)
            {
                var abschnitt = AbschnittParser.Parse(param.Von, param.Bis, param.AbschnittTyp);

                if (null != abschnitt)
                {
                    request.Abschnitt = abschnitt;
                }
            }

            if (!String.IsNullOrWhiteSpace(param.Kundmachungsorgan))
            {
                request.Kundmachungsorgan = new Req.PhraseSearchExpression()
                                                    {
                                                        Value = param.Kundmachungsorgan
                                                    };
            }

            if (!String.IsNullOrWhiteSpace(param.KundmachungsorganNummer))
            {
                request.Kundmachungsorgannummer = new Req.PhraseSearchExpression()
                                                      {
                                                          Value = param.KundmachungsorganNummer
                                                      };
            }

            if (!String.IsNullOrWhiteSpace(param.Index))
            {
                request.Index = QueryParser.Parse(param.Index);
            }

            if (param.Unterzeichnungsdatum.HasValue)
            {
                // Does not exist on Service interface
                throw new NotImplementedException();
            }

            if (param.FassungVom.HasValue)
            {
                request.FassungVom = param.FassungVom.Value.Date;
                request.FassungVomSpecified = true;
            }

            return request;
        }

        public async Task<SearchResult> QueryAsync(RisQueryParameter param, int seitenNummer)
        {
            Req.T_OGDSearchRequest request = null;

            if (param is RisFulltextQueryParameter)
            {
                request = PrepareFulltextSearch((RisFulltextQueryParameter)param);
            }
            else if (param is RisAdvancedQueryParameter)
            {
                request = PrepareAdvancedSearch((RisAdvancedQueryParameter)param);
            }

            if (null == request)
                return new SearchResult("Kein Query Processor gefunden");

            request.ImRisSeitSpecified = true;
            request.ImRisSeit = Mapper.MapChangedWithinToChangesetInterval(param.ImRisSeit);

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

            // We do continuous loading in the UI, thus the user cannot specify the page size
            request.DokumenteProSeiteSpecified = true;
            request.DokumenteProSeite = Req.PageSize.Fifty;

            try
            {
                string requestAsString = MessageSerializationHelper.SerializeToString(request);

                var client = CreateServiceClient();
                requestResponse response = await client.requestAsync("Br", requestAsString);

                var searchResult = MessageSerializationHelper.DeserializeFromString<Resp.T_OGDSearchResult>(response.Body.requestResult);
                return Mapper.MapSearchResult(searchResult);
            }
            catch (Exception ex)
            {
                return new SearchResult(ex.ToString());
            }
        }

        public async Task<DocumentResult> GetDocumentAsync(string dokumentNummer)
        {
            var client = CreateServiceClient();

            try
            {
                getDocumentResponse doc = await client.getDocumentAsync("Br", dokumentNummer);

                var documentResult = MessageSerializationHelper.DeserializeFromString<Doc.DocumentResult>(doc.Body.getDocumentResult);
                return Mapper.MapDocumentResult(documentResult);
            }
            catch (Exception ex)
            {
                return new DocumentResult(ex.ToString());
            }
        }
    }
}
