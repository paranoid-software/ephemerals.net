using System;
using System.Collections.Generic;
using System.Linq;

namespace paranoid.software.ephemerals.MongoDB
{
    public class EphemeralMongoDbContextBuilder: IEphemeralMongoDbContextBuilder
    {
        
        private readonly IFilesManager _filesManager;
        private readonly Dictionary<string, List<dynamic>> _data;
        
        public EphemeralMongoDbContextBuilder() : this(new FilesManager())
        {
            _data = new Dictionary<string, List<dynamic>>();
        }
        
        public EphemeralMongoDbContextBuilder(IFilesManager filesManager)
        {
            _filesManager = filesManager;
            _data = new Dictionary<string, List<dynamic>>();
        }

        public IEphemeralMongoDbContextBuilder AddItemsFromFile(string filepath)
        {
            var fileContent = _filesManager.ReadAllText(filepath);
            if (!fileContent.TryToParseJsonString(out var data)) 
                throw new Exception($"File content for {filepath} is not valid !");
            foreach (var kvp in data)
            {
                if (!_data.ContainsKey(kvp.Key))
                    _data.Add(kvp.Key, new List<dynamic>());
                _data[kvp.Key].AddRange(kvp.Value);
            }
            return this;
        }
        
        public IEphemeralMongoDbContextBuilder AddItems(string collName, IEnumerable<dynamic> items)
        {
            if (!_data.ContainsKey(collName))
                _data.Add(collName, new List<dynamic>());
            _data[collName].AddRange(items);
            return this;
        }
        
        public IEphemeralMongoDbContext Build(ConnectionParams connectionParams, string dbName = null, IDbManager dbManager = null)
        {
            var supportedHostNames = new[] { "localhost", "127.0.0.1" };
            if (!supportedHostNames.Contains(connectionParams.HostName))
                throw new Exception("Ephemeral database must be local, use localhost or 127.0.0.1 as host name.");

            var unsupportedDbNames = new[] { "admin", "config", "local" };
            if (unsupportedDbNames.Contains(dbName)) 
                throw new Exception($"Database name {dbName} is not allowed !");

            return new EphemeralMongoDbContext(dbName, _data, dbManager ?? new DbManager(connectionParams));
        }
        
    }
}