using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace paranoid.software.ephemerals.PostgreSql
{
    public class EphemeralPostgreSqlDbContext: IEphemeralPostgreSqlDbContext
    {
        private readonly IDbManager _dbManager;
        public string DbName { get; private set; }
        public List<Exception> ScriptsErrors { get; private set; }

        public EphemeralPostgreSqlDbContext(string serverConnectionString, List<string> scripts, IDbManager dbManager)
        {
            _dbManager = dbManager;
            InitWith(serverConnectionString, scripts);
        }

        private void InitWith(string serverConnectionString, List<string> scripts)
        {
            var dataSource = new NpgsqlConnectionStringBuilder(serverConnectionString);
            var supportedDataSources = new[] { "localhost", "127.0.0.1" };
            if (!supportedDataSources.Any(ds => dataSource.Host != null && dataSource.Host.Contains(ds)))
                throw new Exception(
                    "Ephemeral database server must be local, use localhost or 127.0.0.1 as server address.");
            if (!string.IsNullOrEmpty(dataSource.Database))
                throw new Exception(
                    "Ephemeral database name should not be included on the connection string, please remove Initial Catalog parameter.");
            
            DbName = $"edb_{Guid.NewGuid().ToString().Replace("-", "")}";

            _dbManager.CreateDatabase(DbName);
            ScriptsErrors = new List<Exception>();

            foreach (var sentence in scripts)
            {
                try
                {
                    _dbManager.ExecuteNonQuery(sentence, DbName);
                }
                catch (Exception e)
                {
                    ScriptsErrors.Add(e);
                }
            }

        }
        
        public IEnumerable<string> GetAllDatabaseNames()
        {
            var queryResult =  _dbManager.ExecuteQuery("SELECT datname as name FROM pg_database;", "");
            return queryResult.Select(x => x["name"].ToString()).ToArray();
        }

        public IEnumerable<string> GetAllTableNames()
        {
            var queryResult =  _dbManager.ExecuteQuery("SELECT tablename as name FROM pg_catalog.pg_tables;", DbName);
            return queryResult.Select(x => x["name"].ToString()).ToArray();
        }

        public int GetRowCount(string tableName)
        {
            var queryResult =  _dbManager.ExecuteQuery($"SELECT count(*) as row_count FROM {tableName};", DbName);
            var queryResultAsArray = queryResult.Select(x => Convert.ToInt32(x["row_count"])).ToArray();
            return queryResultAsArray.Length == 0 ? 0 : queryResultAsArray[0];
        }

        public void Dispose()
        {
            _dbManager.DropDatabase(DbName);
        }
    }
}