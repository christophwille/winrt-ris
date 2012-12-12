using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data.Models;
using SQLite;

namespace Ris.Data
{
    public class RisDbContext
    {
        private const string DatabaseName = "rislocalcache.sqlite";

        private static SQLiteAsyncConnection CreateConnection()
        {
            return new SQLiteAsyncConnection(DatabaseName);
        }

        public static async Task InitializeDatabaseAsync()
        {
            var db = CreateConnection();
            CreateTablesResult result = await db.CreateTableAsync<DbRisQueryParameter>();
            result = await db.CreateTableAsync<DbDownloadedDocument>();
        }

        private readonly SQLiteAsyncConnection _connection;
        public RisDbContext()
        {
            _connection = CreateConnection();
        }

        public async Task InsertSearchHistoryEntry(DbRisQueryParameter rqp)
        {
            int result = await _connection.InsertAsync(rqp);
        }

        public async Task DeleteSearchHistoryEntry(DbRisQueryParameter rqp)
        {
            int result = await _connection.DeleteAsync(rqp);
        }

        public async Task<List<DbRisQueryParameter>> GetSearchHistoryEntries()
        {
            var query = _connection.Table<DbRisQueryParameter>().OrderByDescending(dqp => dqp.Executed);
            var history = await query.ToListAsync();

            return history;
        }

        public async Task<List<string>> GetHistoryEntriesStartingWith(string startsWith)
        {
            var query = _connection.Table<DbRisQueryParameter>()
                .Where(rqp => rqp.FulltextSearchString.StartsWith(startsWith));

            var matched = await query.ToListAsync();

            return matched.Select(rqp => rqp.FulltextSearchString).Distinct().ToList();
        }

        public async Task DeleteSearchHistory()
        {
            var result = await _connection.ExecuteAsync("DELETE FROM DbRisQueryParameter");
        }


        // Downloaded documents

        public async Task<DbDownloadedDocument> GetDownload(int id)
        {
            var doc = await _connection.Table<DbDownloadedDocument>().Where(d => d.Id == id).FirstOrDefaultAsync();
            return doc;
        }

        public async Task InsertDownload(DbDownloadedDocument doc)
        {
            int result = await _connection.InsertAsync(doc);
        }

        public async Task DeleteDownload(DbDownloadedDocument doc)
        {
            int result = await _connection.DeleteAsync(doc);
        }

        public async Task DeleteDownload(int id)
        {
            int result = await _connection.DeleteAsync(new DbDownloadedDocument() { Id = id });
        }

        public async Task RefreshDownload(DbDownloadedDocument doc, int idToReplace)
        {
            // TODO: do it properly in a transaction *or* use InsertOrReplace
            await DeleteDownload(idToReplace);
            await InsertDownload(doc);
        }

        public async Task<List<DbDownloadedDocument>> GetDownloads()
        {
            var query = _connection.Table<DbDownloadedDocument>().OrderByDescending(doc => doc.LastDownloaded);
            var history = await query.ToListAsync();

            return history;
        }

        public async Task DeleteDownloads()
        {
            var result = await _connection.ExecuteAsync("DELETE FROM DbDownloadedDocument");
        }
    }
}
