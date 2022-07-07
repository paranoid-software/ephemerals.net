using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace paranoid.software.ephemerals.MsSql
{
    public class EphemeralMsSqlDbContext : IEphemeralMsSqlDbContext
    {

        private readonly IDbManager _dbManager;
        private readonly IFilesManager _filesManager;
        private readonly List<string> _scripts;

        public string DbName { get; private set; }

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

        public IEphemeralMsSqlDbContext SetDatabaseName(string name)
        {
            DbName = name;
            return this;
        }

        public IEphemeralMsSqlDbContext AddScriptFromFile(string filePath)
        {
            _scripts.Add(_filesManager.ReadAllText(filePath));
            return this;
        }

        public IEphemeralMsSqlDbContext AddScript(string sentence)
        {
            _scripts.Add(sentence);
            return this;
        }

        public IEphemeralMsSqlDbContext Build()
        {
            if (string.IsNullOrEmpty(DbName))
            {
                DbName = $"eph_{Guid.NewGuid().ToString().Replace("-", "")}";
            }

            _dbManager.CreateDatabase(DbName);

            foreach (var sentence in _scripts)
            {
                _dbManager.ExecuteNonQuery(sentence, DbName);
            }

            return this;
        }

        public IEnumerable<string> GetAllDatabaseNames()
        {
            return _dbManager.GetAllDatabaseNames();
        }

        public void Dispose()
        {
            _dbManager.DropDatabase(DbName);
        }
    }
}