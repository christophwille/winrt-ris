using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ris.Client.Models;

using Req = Ris.Client.Messages.Request;
using Resp = Ris.Client.Messages.Response;
using Doc = Ris.Client.Messages.Document;
using Ris.Client.Messages;

namespace Ris.Client
{
    public static class Mapper
    {
        public static SearchResult MapSearchResult(Resp.T_OGDSearchResult searchResult)
        {
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

        public static DocumentResult MapDocumentResult(Doc.DocumentResult documentResult)
        {
            if (documentResult.status == Doc.DocumentResultStatus.error)
            {
                var error = (Doc.T_Error)documentResult.Item;
                return new DocumentResult(error.Message);
            }

            var documentsResult = (Doc.T_OGDWebDocument)documentResult.Item;

            var transformedDocument = new Client.Models.Document()
                                          {
                                              Abkuerzung = documentsResult.Abkuerzung,
                                              Aenderung = documentsResult.Aenderung,
                                              Aenderungsdatum = documentsResult.Aenderungsdatum,
                                              AlteDokumentnummer = documentsResult.AlteDokumentnummer,
                                              Anmerkung = documentsResult.Anmerkung,
                                              AnmerkungZurGanzenRechtsvorschrift =
                                                  documentsResult.AnmerkungZurGanzenRechtsvorschrift,
                                              ArtikelParagraphAnlage = documentsResult.ArtikelParagraphAnlage,
                                              Ausserkrafttretedatum = documentsResult.Ausserkrafttretedatum,
                                              Beachte = documentsResult.Beachte,
                                              BeachteZurGanzenRechtsvorschrift =
                                                  documentsResult.BeachteZurGanzenRechtsvorschrift,
                                              Dokumentnummer = documentsResult.Dokumentnummer,
                                              Gesetzesnummer = documentsResult.Gesetzesnummer,

                                              Indizes = documentsResult.Indizes != null
                                                            ? new List<string>(documentsResult.Indizes)
                                                            : new List<string>(),

                                              Inkrafttretedatum = documentsResult.Inkrafttretedatum,
                                              Kundmachungsorgan = documentsResult.Kundmachungsorgan,
                                              Kurztitel = documentsResult.Kurztitel,
                                              Langtitel = documentsResult.Langtitel,
                                              Schlagworte = documentsResult.Schlagworte,
                                              Sprachen = documentsResult.Sprachen,
                                              Staaten = documentsResult.Staaten,
                                              Typ = documentsResult.Typ,
                                              Uebergangsrecht = documentsResult.Uebergangsrecht,
                                              Unterzeichnungsdatum = documentsResult.Unterzeichnungsdatum,
                                              Veroeffentlichungsdatum = documentsResult.Veroeffentlichungsdatum,
                                          };

            var transformedContentItems = new List<Client.Models.DocumentContent>();

            foreach (Doc.T_WebDocumentContentReference content in documentsResult.Dokumentinhalt)
            {
                DocumentContentTypeEnum ctype = ContentTypeToContentTypeEnum(content.ContentType);
                DocumentContentDataTypeEnum dtype = DataTypeToDataTypeEnum(content.DataType);

                if (content.Item is Doc.risdok)
                {
                    var risdok = (Doc.risdok)content.Item;

                    if (null != risdok.nutzdaten)
                    {
                        var transformedContent = new Client.Models.DocumentContent()
                                                     {
                                                         Name = content.Name,
                                                         ContentType = ctype,
                                                         DataType = dtype,
                                                         Nutzdaten = risdok.nutzdaten.Text, // for Xslt processing
                                                     };

                        transformedContentItems.Add(transformedContent);
                    }
                }
                else
                {
                    // content.Item is a byte[] when correctly decoded, eg see risdok://NOR12088695
                    if (content.Item is byte[])
                    {
                        var attachment = new Client.Models.DocumentContent()
                        {
                            Name = content.Name,    // this Name property is sent extensionless
                            ContentType = ctype,
                            DataType = dtype,
                            Content = (byte[])content.Item
                        };

                        transformedContentItems.Add(attachment);
                    }
                }
            }

            return new DocumentResult()
                       {
                           Succeeded = true,
                           Document = transformedDocument,
                           DocumentContents = transformedContentItems
                       };
        }

        private static DocumentContentDataTypeEnum DataTypeToDataTypeEnum(Doc.T_WebDocumentDataType orig)
        {
            DocumentContentDataTypeEnum outVar;

            switch (orig)
            {
                case Doc.T_WebDocumentDataType.Gif:
                    outVar = DocumentContentDataTypeEnum.Gif;
                    break;
                case Doc.T_WebDocumentDataType.Jpg:
                    outVar = DocumentContentDataTypeEnum.Jpg;
                    break;
                case Doc.T_WebDocumentDataType.Pdf:
                    outVar = DocumentContentDataTypeEnum.Pdf;
                    break;
                case Doc.T_WebDocumentDataType.Png:
                    outVar = DocumentContentDataTypeEnum.Png;
                    break;
                case Doc.T_WebDocumentDataType.Tiff:
                    outVar = DocumentContentDataTypeEnum.Tiff;
                    break;
                default:
                    outVar = DocumentContentDataTypeEnum.Xml;
                    break;
            }

            return outVar;
        }

        private static DocumentContentTypeEnum ContentTypeToContentTypeEnum(Doc.T_WebDocumentContentType orig)
        {
            if (Doc.T_WebDocumentContentType.MainDocument == orig)
                return DocumentContentTypeEnum.MainDocument;

            return DocumentContentTypeEnum.Attachment;
        }

        public static ChangedWithinEnum MapChangesetIntervalToChangedWithin(Req.ChangeSetInterval changeSetInterval)
        {
            switch (changeSetInterval)
            {
                case Req.ChangeSetInterval.Undefined:
                    return ChangedWithinEnum.Undefined;
                    break;
                case Req.ChangeSetInterval.EinerWoche:
                    return ChangedWithinEnum.EinerWoche;
                    break;
                case Req.ChangeSetInterval.ZweiWochen:
                    return ChangedWithinEnum.ZweiWochen;
                    break;
                case Req.ChangeSetInterval.EinemMonat:
                    return ChangedWithinEnum.EinemMonat;
                    break;
                case Req.ChangeSetInterval.DreiMonaten:
                    return ChangedWithinEnum.DreiMonaten;
                    break;
                case Req.ChangeSetInterval.SechsMonaten:
                    return ChangedWithinEnum.SechsMonaten;
                    break;
                case Req.ChangeSetInterval.EinemJahr:
                    return ChangedWithinEnum.EinemJahr;
                    break;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static Req.ChangeSetInterval MapChangedWithinToChangesetInterval(ChangedWithinEnum changedWithin)
        {
            switch (changedWithin)
            {
                case ChangedWithinEnum.Undefined:
                    return Req.ChangeSetInterval.Undefined;
                    break;
                case ChangedWithinEnum.EinerWoche:
                    return Req.ChangeSetInterval.EinerWoche;
                    break;
                case ChangedWithinEnum.ZweiWochen:
                    return Req.ChangeSetInterval.ZweiWochen;
                    break;
                case ChangedWithinEnum.EinemMonat:
                    return Req.ChangeSetInterval.EinemMonat;
                    break;
                case ChangedWithinEnum.DreiMonaten:
                    return Req.ChangeSetInterval.DreiMonaten;
                    break;
                case ChangedWithinEnum.SechsMonaten:
                    return Req.ChangeSetInterval.SechsMonaten;
                    break;
                case ChangedWithinEnum.EinemJahr:
                    return Req.ChangeSetInterval.EinemJahr;
                    break;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static Req.NormabschnittTyp MapAbschnittTypToNormabschnittTyp(AbschnittTypEnum abschnitt)
        {
            switch (abschnitt)
            {
                case AbschnittTypEnum.Alle:
                    return Req.NormabschnittTyp.Alle;
                    break;
                case AbschnittTypEnum.Artikel:
                    return Req.NormabschnittTyp.Artikel;
                    break;
                case AbschnittTypEnum.Paragraph:
                    return Req.NormabschnittTyp.Paragraph;
                    break;
                case AbschnittTypEnum.Anlage:
                    return Req.NormabschnittTyp.Anlage;
                    break;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static string MapDocumentContentDataTypeEnumToExtension(DocumentContentDataTypeEnum type)
        {
            return type.ToString();
        }
    }
}
