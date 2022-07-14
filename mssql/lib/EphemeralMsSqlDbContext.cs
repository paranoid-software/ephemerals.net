using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using paranoid.software.ephemerals.MsSql.paranoid.software.ephemerals.MsSql;

namespace paranoid.software.ephemerals.MsSql
{
    public class EphemeralMsSqlDbContext : IEphemeralMsSqlDbContextBuilder, IEphemeralMsSqlDbContext
    {

        private readonly IDbManager _dbManager;
        private readonly IFilesManager _filesManager;
        private readonly List<string> _scripts;

        public string DbName { get; private set; }
        public List<Exception> ScriptsErrors { get; private set; }

        public EphemeralMsSqlDbContext(string serverConnectionString) : this(serverConnectionString,
            new DbManager(serverConnectionString), new FilesManager())
        {
        }

        public EphemeralMsSqlDbContext(string serverConnectionString, IDbManager dbManager, IFilesManager filesManager)
        {
            var dataSource = new SqlConnectionStringBuilder(serverConnectionString);
            var supportedDataSources = new[] { "localhost", "127.0.0.1" };
            if (!supportedDataSources.Any(ds => dataSource.DataSource.Contains(ds)))
                throw new Exception(
                    "Ephemeral database server must be local, use localhost or 127.0.0.1 as server address.");
            if (!string.IsNullOrEmpty(dataSource.InitialCatalog))
                throw new Exception(
                    "Ephemeral database name should not be included on the connection string, please remove Initial Catalog parameter.");
            _dbManager = dbManager;
            _filesManager = filesManager;
            _scripts = new List<string>();
        }
        
        public IEphemeralMsSqlDbContextBuilder AddScriptFromFile(string filePath)
        {
            _scripts.Add(_filesManager.ReadAllText(filePath));
            return this;
        }

        public IEphemeralMsSqlDbContextBuilder AddScript(string sentence)
        {
            _scripts.Add(sentence);
            return this;
        }

        public IEphemeralMsSqlDbContext Build()
        {
            DbName = $"edb_{Guid.NewGuid().ToString().Replace("-", "")}";

            _dbManager.CreateDatabase(DbName);
            ScriptsErrors = new List<Exception>();

            foreach (var sentence in _scripts)
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

            return this;
        }

        public IEnumerable<string> GetAllDatabaseNames()
        {
            var queryResult =  _dbManager.ExecuteQuery("SELECT name FROM sys.databases;", "master");
            return queryResult.Select(x => x["name"].ToString()).ToArray();
        }

        public IEnumerable<string> GetAllTableNames()
        {
            var queryResult =  _dbManager.ExecuteQuery("SELECT name FROM sys.tables;", DbName);
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