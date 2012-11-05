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

        public async Task<List<DbRisQueryParameter>> GetTenMostRecentSearchHistoryEntries()
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
    }
}
