using System;
using System.Collections.Generic;

namespace paranoid.software.ephemerals.MongoDB
{
    public class EphemeralMongoDbContext : IEphemeralMongoDbContext
    {
        private readonly IDbManager _dbManager;
        
        public string DbName { get; private set; }
        public List<Exception> InitializationErrors { get; }

        public EphemeralMongoDbContext(string dbName, Dictionary<string, List<dynamic>> data, IDbManager dbManager)
        {
            _dbManager = dbManager;
            InitializationErrors = new List<Exception>();
            InitDbWith(dbName, data);
        }

        private void InitDbWith(string dbName, Dictionary<string, List<dynamic>> data)
        {   
            if (dbName is null)
            {
                DbName = $"edb_{Guid.NewGuid():N}";
            }
            else
            {
                if (_dbManager.DatabaseExists(dbName))
                    throw new Exception($"Database name {dbName} is not available !");
                DbName = dbName;
            }
            
            _dbManager.CreateDatabase(DbName);

            foreach (var kvp in data)
            {
                foreach (var item in kvp.Value)
                {
                    try
                    {
                        _dbManager.InsertDocument(DbName, kvp.Key, (object)item);
                    }
                    catch (Exception ex)
                    {
                        InitializationErrors.Add(ex);
                    }
                }
            }
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            return _dbManager.GetDatabaseNames();
        }

        public IEnumerable<string> GetCollectionNames()
        {
            return _dbManager.GetCollectionNames(DbName);
        }

        public long GetDocumentsQty(string collName)
        {
            return _dbManager.GetDocumentsQty(DbName, collName);
        }

        public void Dispose()
        {
            _dbManager.DropDatabase(DbName);
        }

    }
}