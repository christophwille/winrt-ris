using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Client;
using Ris.Client.Messages;
using Ris.Client.Models;
using Doc = Ris.Client.Messages.Document;

namespace RisCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Verwendung: RisCmd NORxxxxx");
            }

            string dokumentNummer = args[0];

            var task = new Task<DocumentResult>(() => DocumentLoadTask.LoadAsync(dokumentNummer).Result);
            task.Start();
            task.Wait();

            var result = task.Result;

            if (result.Succeeded)
            {
                string fileName = dokumentNummer + ".xml";
                File.WriteAllText(fileName, result.OriginalDocumentResultXml);

                Console.WriteLine(fileName + " erfolgreich lokal gespeichert");
            }
            else
            {
                Console.WriteLine("Dokument konnte nicht geladen werden, Fehlermeldung: ");
                Console.WriteLine(result.Error);
            }
        }
    }

    static class DocumentLoadTask
    {
        public static async Task<DocumentResult> LoadAsync(string dokumentNummer)
        {
            var client = new OGDServiceSoapClient();

            try
            {
                getDocumentResponse doc = await client.getDocumentAsync("Br", dokumentNummer);

                var documentResult = MessageSerializationHelper.DeserializeFromString<Doc.DocumentResult>(doc.Body.getDocumentResult);
                var result = Mapper.MapDocumentResult(documentResult);

                result.OriginalDocumentResultXml = doc.Body.getDocumentResult;
                return result;
            }
            catch (Exception ex)
            {
                return new DocumentResult(ex.ToString());
            }
        }
    }
}
